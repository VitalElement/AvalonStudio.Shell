using System;
using System.Diagnostics;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.CreateNugetPackages);

	BuildParameters Parameters { get; set; }

	[Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";

	protected override void OnBuildInitialized()
	{
		base.OnBuildInitialized();

		Parameters = new BuildParameters(this);
		Information("Building version {0} of AvalonStudio.Shell ({1}) using version {2} of Nuke.",
			Parameters.Version,
			Parameters.Configuration,
			typeof(NukeBuild).Assembly.GetName().Version.ToString());

		if (Parameters.IsLocalBuild)
		{
			Information("Repository Name: " + Parameters.RepositoryName);
			Information("Repository Branch: " + Parameters.RepositoryBranch);
		}
		Information("Configuration: " + Parameters.Configuration);
		Information("IsLocalBuild: " + Parameters.IsLocalBuild);
		Information("IsRunningOnUnix: " + Parameters.IsRunningOnUnix);
		Information("IsRunningOnWindows: " + Parameters.IsRunningOnWindows);
		Information("IsRunningOnAzure:" + Parameters.IsRunningOnAzure);
		Information("IsPullRequest: " + Parameters.IsPullRequest);
		Information("IsMainRepo: " + Parameters.IsMainRepo);
		Information("IsMasterBranch: " + Parameters.IsMasterBranch);
		Information("IsReleaseBranch: " + Parameters.IsReleaseBranch);
		Information("IsReleasable: " + Parameters.IsReleasable);
		Information("IsMyGetRelease: " + Parameters.IsMyGetRelease);
		Information("IsNuGetRelease: " + Parameters.IsNuGetRelease);

		void ExecWait(string preamble, string command, string args)
		{
			Console.WriteLine(preamble);
			Process.Start(new ProcessStartInfo(command, args) { UseShellExecute = false }).WaitForExit();
		}
		ExecWait("dotnet version:", "dotnet", "--version");
		if (Parameters.IsRunningOnUnix)
			ExecWait("Mono version:", "mono", "--version");
	}

	Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
			DeleteDirectories(Parameters.BuildDirs);
			EnsureCleanDirectories(Parameters.BuildDirs);
			EnsureCleanDirectory(Parameters.ArtifactsDir);
			EnsureCleanDirectory(Parameters.NugetIntermediateRoot);
			EnsureCleanDirectory(Parameters.NugetRoot);
			EnsureCleanDirectory(Parameters.ZipRoot);
			EnsureCleanDirectory(Parameters.TestResultsRoot);
			EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

	Target CreateIntermediateNugetPackages => _ => _
	.DependsOn(Compile)
	.Executes(() =>
	{
		EnsureCleanDirectory(Parameters.NugetIntermediateRoot);

		DotNetPack(Solution, x =>
		x.SetConfiguration(Configuration)
		.SetOutputDirectory(Parameters.NugetIntermediateRoot)
		.AddProperty("PackageVersion", Parameters.Version));
	});

	Target CreateNugetPackages => _ => _
		.DependsOn(CreateIntermediateNugetPackages)
	
		.Executes(() =>
		{
			var logger = new NumergeNukeLogger();
			var config = Numerge.MergeConfiguration.LoadFile(RootDirectory / "build" / "numerge.config");
			EnsureCleanDirectory(Parameters.NugetRoot);
			if (!Numerge.NugetPackageMerger.Merge(Parameters.NugetIntermediateRoot, Parameters.NugetRoot, config,
				logger))
				throw new Exception("Package merge failed");
		});

	Target CiAzureWindows => _ => _
		.DependsOn(CreateNugetPackages);
}
