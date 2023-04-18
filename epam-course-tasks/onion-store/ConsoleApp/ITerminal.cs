using System;
using System.Collections.Generic;
using OnionCore;

namespace ConsoleApp
{
    /// <summary>
    /// Terminal interface.
    /// </summary>
    public interface ITerminal
    {
        /// <summary>
        /// A dictionary of commands and corresponding delegates that are available to the current <c>IUser</c>.
        /// </summary>
        public IDictionary<string, Delegate> Commands { get; set; }
        
        /// <summary>
        /// The current <c>IUser</c>.
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// The main loop of the terminal workflow.
        /// </summary>
        public void Start();
    }
}