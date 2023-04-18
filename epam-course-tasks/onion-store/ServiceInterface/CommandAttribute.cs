using System;

namespace ServiceInterface
{
    /// <summary>
    /// The attribute used for specifying commands (the names that will be used in the terminal).
    /// </summary>
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Command name.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="command">Command name.</param>
        public CommandAttribute(string command)
        {
            Command = command;
        }
    }
}