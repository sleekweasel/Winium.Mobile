namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Collections.Generic;

    using Winium.Mobile.Common;
    using Mobile.Driver.Automator;

    #endregion

    internal class GetWindowSizeExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var phoneScreenSize = this.Automator.EmulatorController.PhoneScreenSize;

            return this.JsonResponse(
                ResponseStatus.Success, 
                new Dictionary<string, int> {
                    { "width", (int)(phoneScreenSize.Width / Capabilities.Scale) },
                    { "height", (int)(phoneScreenSize.Height / Capabilities.Scale) }
                });
        }

        #endregion
    }
}
