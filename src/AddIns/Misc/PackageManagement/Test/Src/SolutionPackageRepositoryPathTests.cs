﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SolutionPackageRepositoryPathTests
	{
		SolutionPackageRepositoryPath repositoryPath;
		IProject testProject;
		PackageManagementOptions options;
		ISolution solution;
		
		void CreateSolutionPackageRepositoryPath()
		{
			repositoryPath = new SolutionPackageRepositoryPath(testProject, options);
		}
		
		void CreateSolutionPackageRepositoryPath(ISolution solution)
		{
			repositoryPath = new SolutionPackageRepositoryPath(solution, options);
		}
		
		void CreateTestProject()
		{
			testProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateSolution(string fileName)
		{
			solution = MockRepository.GenerateStrictMock<ISolution>();
			var file = FileName.Create(fileName);
			solution.Stub(s => s.FileName).Return(file);
			solution.Stub(s => s.Directory).Return(file.GetParentDirectory());
		}
		
		void CreateOptions()
		{
			options = new TestablePackageManagementOptions();
		}
		
		[Test]
		public void PackageRepositoryPath_ProjectAndSolutionHaveDifferentFolders_IsConfiguredPackagesFolderInsideSolutionFolder()
		{
			CreateOptions();
			CreateTestProject();
			testProject.ParentSolution.Stub(s => s.Directory).Return(DirectoryName.Create(@"d:\projects\MyProject\"));
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath();
			
			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MyProject\MyPackages";
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void PackageRepositoryPath_PassSolutionToConstructor_IsConfiguredPackagesFolderInsideSolutionFolder()
		{
			CreateOptions();
			CreateSolution(@"d:\projects\MySolution\MySolution.sln");
			options.PackagesDirectory = "Packages";
			CreateSolutionPackageRepositoryPath(solution);
			
			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MySolution\Packages";
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void GetInstallPath_GetInstallPathForPackage_ReturnsPackagePathInsideSolutionPackagesRepository()
		{
			CreateOptions();
			CreateSolution(@"d:\projects\Test\MySolution\MyProject.sln");
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath(solution);
			
			var package = FakePackage.CreatePackageWithVersion("MyPackage", "1.2.1.40");
			
			string installPath = repositoryPath.GetInstallPath(package);
			
			string expectedInstallPath = 
				@"d:\projects\Test\MySolution\MyPackages\MyPackage.1.2.1.40";
			
			Assert.AreEqual(expectedInstallPath, installPath);
		}
	}
}
