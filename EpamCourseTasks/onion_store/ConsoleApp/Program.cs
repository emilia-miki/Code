using ServiceInterface.Controllers;
using MockLibrary;

namespace ConsoleApp
{
    /// <summary>
    /// The Program class.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main method. Creates a <c>MainController</c> and starts a terminal with guest delegates.
        /// </summary>
        public static void Main()
        {
            var controller = new MainController(
                MockIUserRepository.GetMock().Object, 
                MockIProductRepository.GetMock().Object,
                MockIOrderRepository.GetMock().Object
                );
            
            var terminal = new Terminal
            {
                Commands = controller.GetDelegates()
            };
            terminal.Start();
        }
    }
}
