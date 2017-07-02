﻿using System.Linq;
using System.Runtime.Versioning;
using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    using CShell.Framework.Services;

    public class InstallCommand : IReplCommand
    {
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPackageAssemblyResolver _packageAssemblyResolver;

        private readonly IReplOutput _replOutput;

        private readonly IInstallationProvider _installationProvider;

        public InstallCommand(
            IPackageInstaller packageInstaller,
            IPackageAssemblyResolver packageAssemblyResolver,
            IReplOutput replOutput,
            IInstallationProvider installationProvider)
        {
            _packageInstaller = packageInstaller;
            _packageAssemblyResolver = packageAssemblyResolver;
            _replOutput = replOutput;
            _installationProvider = installationProvider;
        }

        public string CommandName
        {
            get { return "install"; }
        }

        public string Description
        {
            get { return "Installs a Nuget package. I.e. :install <package> <version> <\"pre\">"; }
        }

        public object Execute(IRepl repl, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            string version = null;
            var allowPre = false;
            if (args.Length >= 2)
            {
                version = args[1].ToString();
            }

            allowPre = args.Length >= 3 && args[2].ToString().ToUpperInvariant() == "PRE";

            _replOutput.WriteLine("Installing {0}", args[0]);

            _installationProvider.Initialize();

            var packageRef = new PackageReference(
                args[0].ToString(), new FrameworkName(Constants.NetFrameworkName), version);

            _packageInstaller.InstallPackages(new[] { packageRef }, allowPre);
            _packageAssemblyResolver.SavePackages();

            var dlls = _packageAssemblyResolver.GetAssemblyNames(repl.FileSystem.CurrentDirectory)
                .Except(repl.References.Paths).ToArray();

            repl.AddReferences(dlls);

            foreach (var dll in dlls)
            {
                _replOutput.WriteLine("Added reference to {0}", dll);
            }

            return null;
        }
    }
}
