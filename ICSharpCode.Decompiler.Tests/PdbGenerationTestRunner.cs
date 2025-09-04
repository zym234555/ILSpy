using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;

using Microsoft.DiaSymReader.Tools;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class PdbGenerationTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/PdbGen";

		[Test]
		public void HelloWorld()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Missing nested local scopes for loops, differences in IL ranges")]
		public void ForLoopTests()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Differences in IL ranges")]
		public void LambdaCapturing()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Duplicate sequence points for local function")]
		public void Members()
		{
			TestGeneratePdb();
		}

		[Test]
		public void CustomPdbId()
		{
			// Generate a PDB for an assembly using a randomly-generated ID, then validate that the PDB uses the specified ID
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(CustomPdbId));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			var expectedPdbId = new BlobContentId(Guid.NewGuid(), (uint)Random.Shared.Next());

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(CustomPdbId) + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				PortablePdbWriter.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream, noLogo: true, pdbId: expectedPdbId);

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);

				Assert.That(generatedPdbId.Guid, Is.EqualTo(expectedPdbId.Guid));
				Assert.That(generatedPdbId.Stamp, Is.EqualTo(expectedPdbId.Stamp));
			}
		}

		[Test]
		public void ProgressReporting()
		{
			// Generate a PDB for an assembly and validate that the progress reporter is called with reasonable values
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(ProgressReporting));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());

			var lastFilesWritten = 0;
			var totalFiles = -1;

			Action<DecompilationProgress> reportFunc = progress => {
				if (totalFiles == -1)
				{
					// Initialize value on first call
					totalFiles = progress.TotalUnits;
				}

				Assert.That(totalFiles, Is.EqualTo(progress.TotalUnits));
				Assert.That(lastFilesWritten + 1, Is.EqualTo(progress.UnitsCompleted));

				lastFilesWritten = progress.UnitsCompleted;
			};

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(ProgressReporting) + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				PortablePdbWriter.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream, noLogo: true, progress: new TestProgressReporter(reportFunc));

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);
			}

			Assert.That(lastFilesWritten, Is.EqualTo(totalFiles));
		}

		private class TestProgressReporter : IProgress<DecompilationProgress>
		{
			private Action<DecompilationProgress> reportFunc;

			public TestProgressReporter(Action<DecompilationProgress> reportFunc)
			{
				this.reportFunc = reportFunc;
			}

			public void Report(DecompilationProgress value)
			{
				reportFunc(value);
			}
		}

		private void TestGeneratePdb([CallerMemberName] string testName = null)
		{
			const PdbToXmlOptions options = PdbToXmlOptions.IncludeEmbeddedSources | PdbToXmlOptions.ThrowOnError | PdbToXmlOptions.IncludeTokens | PdbToXmlOptions.ResolveTokens | PdbToXmlOptions.IncludeMethodSpans;

			string xmlFile = Path.Combine(TestCasePath, testName + ".xml");
			(string peFileName, string pdbFileName) = CompileTestCase(testName);

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, testName + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				PortablePdbWriter.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream, noLogo: true);
				pdbStream.Position = 0;
				using (Stream peStream = File.OpenRead(peFileName))
				using (Stream expectedPdbStream = File.OpenRead(pdbFileName))
				{
					using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(pdbFileName, ".xml"), false, Encoding.UTF8))
					{
						PdbToXmlConverter.ToXml(writer, expectedPdbStream, peStream, options);
					}
					peStream.Position = 0;
					using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(xmlFile, ".generated.xml"), false, Encoding.UTF8))
					{
						PdbToXmlConverter.ToXml(writer, pdbStream, peStream, options);
					}
				}
			}
			string expectedFileName = Path.ChangeExtension(xmlFile, ".expected.xml");
			ProcessXmlFile(expectedFileName);
			string generatedFileName = Path.ChangeExtension(xmlFile, ".generated.xml");
			ProcessXmlFile(generatedFileName);
			CodeAssert.AreEqual(Normalize(expectedFileName), Normalize(generatedFileName));
		}

		private (string peFileName, string pdbFileName) CompileTestCase(string testName)
		{
			string xmlFile = Path.Combine(TestCasePath, testName + ".xml");
			string xmlContent = File.ReadAllText(xmlFile);
			XDocument document = XDocument.Parse(xmlContent);
			var files = document.Descendants("file").ToDictionary(f => f.Attribute("name").Value, f => f.Value);
			Tester.CompileCSharpWithPdb(Path.Combine(TestCasePath, testName + ".expected"), files);

			string peFileName = Path.Combine(TestCasePath, testName + ".expected.dll");
			string pdbFileName = Path.Combine(TestCasePath, testName + ".expected.pdb");

			return (peFileName, pdbFileName);
		}

		private void ProcessXmlFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			foreach (var file in document.Descendants("file"))
			{
				file.Attribute("checksum").Remove();
				file.Attribute("embeddedSourceLength")?.Remove();
				file.ReplaceNodes(new XCData(file.Value.Replace("\uFEFF", "")));
			}
			document.Save(fileName, SaveOptions.None);
		}

		private string Normalize(string inputFileName)
		{
			return File.ReadAllText(inputFileName).Replace("\r\n", "\n").Replace("\r", "\n");
		}
	}

	class StringWriterWithEncoding : StringWriter
	{
		readonly Encoding encoding;

		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding ?? throw new ArgumentNullException("encoding");
		}

		public override Encoding Encoding => encoding;
	}
}
