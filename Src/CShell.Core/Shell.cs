﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell
{
    using ScriptCs.Contracts;

    /// <summary>
    /// Hosts all general utility methods to access and modify the shell. 
    /// Use this static class to manage the workspace, open documents, evaluate code and manage or dump to the sinks.
    /// </summary>
    public static partial class Shell
    {
        private static readonly Lazy<IShell> _shellLazy = new Lazy<IShell>(() => IoC.Get<IShell>());
        private static IShell UI { get { return _shellLazy.Value; } }

        /// <summary>
        /// Update the progress in the staus bar.
        /// </summary>
        /// <param name="progress">The progress between 0 and 100.</param>
        public static void UpdateProgress(int progress)
        {
            UI.StatusBar.UpdateProgress(progress);
        }

        #region Evaluate helpers
        /// <summary>
        /// Evaluates a piece of code.
        /// </summary>
        /// <param name="input">The code to evaluate.</param>
        /// <returns>The results of the evaluation.</returns>
        public static ScriptResult Evaluate(string input)
        {
            if (Workspace == null)
                return null;
            return Workspace.ReplExecutor.Execute(input);
        }

        /// <summary>
        /// Evaluates all the code in a file.
        /// </summary>
        /// <param name="filePath">The path to the file. Can be relative to the root path of the workspace, e.g. "subfolder/file.csx"</param>
        /// <returns>The results of the evaluation.</returns>
        public static ScriptResult EvaluateFile(string filePath)
        {
            if (Workspace == null)
                return null;
            if (filePath == null) throw new ArgumentNullException("filePath");
            if (File.Exists(filePath))
            {
                // var fileText = System.IO.File.ReadAllText(filePath);
                return Workspace.ReplExecutor.ExecuteScript(filePath);
            }
            else
                throw new FileNotFoundException("filePath not found.", filePath);
        }

        /// <summary>
        /// Evaluates all the code in a file.
        /// </summary>
        /// <param name="filePath">The path to the file. Can be relative to the root path of the workspace, e.g. "subfolder/file.csx"</param>
        /// <returns><c>true</c> if the evaluation succeeded, otherwise <c>false</c>.</returns>
        public static bool TryEvaluateFile(string filePath)
        {
            if (Workspace == null)
                return false;
            if (File.Exists(filePath))
            {
                return !EvaluateFile(filePath).IsCompleteSubmission;
            }
            return false;
        }

        #endregion
    }
}
