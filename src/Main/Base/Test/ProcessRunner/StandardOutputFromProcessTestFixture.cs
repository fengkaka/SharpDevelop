﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Runs a process that returns some text on the standard output.
	/// </summary>
	[TestFixture]
	public class StandardOutputFromProcessTestFixture : ConsoleAppTestFixtureBase
	{
		/// <summary>
		/// Runs a process expecting a single line of output.
		/// </summary>
		[Test]
		public void SingleLineOfOutput()
		{			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			string echoText = "Test";
			string expectedOutput = "Test\r\n";
			runner.RedirectStandardOutput = true;
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			string output = runner.OpenStandardOutputReader().ReadToEnd();
			
			Assert.AreEqual(expectedOutput, output, "Should have some output.");
		}
		
		/// <summary>
		/// Runs a process and makes sure it does not return any output.
		/// </summary>
		[Test]
		public void NoOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			runner.RedirectStandardOutput = true;
			runner.RedirectStandardError = true;
			
			runner.Start(GetConsoleAppFileName());
			runner.WaitForExit();
			
			//Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
			Assert.AreEqual("", runner.OpenStandardOutputReader().ReadToEnd(), "Should not be any output.");
			Assert.AreEqual("", runner.OpenStandardErrorReader().ReadToEnd(), "Should not be any error output.");			
		}
		/*
		/// <summary>
		/// The process that is run tries to send such a large amount of 
		/// data that it fills the standard output's buffer.
		/// </summary>
		[Test]
		public void LargeAmountOfOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			string filename = "test.txt";
			string fullFilename = Path.Combine(runner.WorkingDirectory, filename);
		
			try
			{
				IEnumerable<string> outputText = GetOutputText();
				File.WriteAllLines(fullFilename, outputText);
				runner.Start(GetConsoleAppFileName(), String.Concat("-file:", filename));
				bool exited = runner.WaitForExitAsync().Wait(5000);
				
				Assert.IsTrue(exited, "App did not exit.");
				Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
				Assert.AreEqual(outputText, runner.StandardOutput, "Should have some output.");
				Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");						
			}
			finally
			{
				if (File.Exists(fullFilename)) {
					File.Delete(fullFilename);
				}
			}
		}

		/// <summary>
		/// The process that is run tries to send such a large amount of 
		/// data that it fills the standard output's buffer.
		/// </summary>
		[Test]
		public void LargeAmountOfErrorOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			string filename = "test.txt";
			string fullFilename = Path.Combine(runner.WorkingDirectory, filename);
		
			try
			{
				IEnumerable<string> outputText = GetOutputText();
				File.WriteAllLines(fullFilename, outputText);
				runner.Start(GetConsoleAppFileName(), String.Concat("-error.file:", filename));
				bool exited = runner.WaitForExitAsync().Wait(5000);
				
				Assert.IsTrue(exited, "App did not exit.");
				Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
				Assert.AreEqual(String.Empty, runner.StandardOutput, "Should not be any output.");
				Assert.AreEqual(outputText, runner.StandardError, "Should have some error output.");						
			}
			finally
			{
				if (File.Exists(fullFilename)) {
					File.Delete(fullFilename);
				}
			}
		}
		*/
		/// <summary>
		/// Runs a process expecting a single line of output written
		/// to standard error.
		/// </summary>
		[Test]
		public void SingleLineOfErrorOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			string echoText = "Test";
			string expectedOutput = "Test\r\n";
			runner.RedirectStandardError = true;
			runner.Start(GetConsoleAppFileName(), String.Concat("-error.echo:", echoText));
			string output = runner.OpenStandardErrorReader().ReadToEnd();
			
			Assert.AreEqual(expectedOutput, output, "Should have some output.");
		}		
		
		/// <summary>
		/// Gets the output text that the console app will
		/// attempt to send us back.
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetOutputText()
		{
			List<string> outputText = new List<string>();
			for (int i = 0; i < 300; ++i) {
				outputText.Add("i=" + i.ToString() + " A line of text.");
			}
			return outputText;
		}
	}
}
