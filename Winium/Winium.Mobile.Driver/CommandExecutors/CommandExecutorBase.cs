namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System;
    using System.Net;

    using Newtonsoft.Json;

    using Mobile.Common;
    using Mobile.Common.Exceptions;
    using Mobile.Driver.Automator;
    using System.Drawing;

    #endregion

    internal class CommandExecutorBase
    {
        #region Public Properties

        public Command ExecutedCommand { get; set; }

        #endregion

        #region Properties

        protected Automator Automator { get; set; }

        #endregion

        #region Public Methods and Operators

        public static Point? Scale(Point? coordinates)
        {
            if (!coordinates.HasValue) return null;
            return new Point(
              (int)(coordinates.Value.X * Capabilities.Scale),
              (int)(coordinates.Value.Y * Capabilities.Scale));
        }

        public static Point Scale(Point coordinates)
        {
            return new Point(
              (int)(coordinates.X * Capabilities.Scale),
              (int)(coordinates.Y * Capabilities.Scale));
        }

        public static int Scale(int ordinate)
        {
            return (int)(ordinate * Capabilities.Scale);
        }

        public static double Scale(double ordinate)
        {
            return ordinate * Capabilities.Scale;
        }

        public CommandResponse Do()
        {
            if (this.ExecutedCommand == null)
            {
                throw new NullReferenceException("ExecutedCommand property must be set before calling Do");
            }

            try
            {
                var session = this.ExecutedCommand.SessionId;
                this.Automator = Automator.InstanceForSession(session);
                return CommandResponse.Create(HttpStatusCode.OK, this.DoImpl());
            }
            catch (AutomationException exception)
            {
                return CommandResponse.Create(HttpStatusCode.OK, this.JsonResponse(exception.Status, exception));
            }
            catch (InnerServerRequestException exception)
            {
                // Bad status returned by Inner Server when trying to forward command
                return CommandResponse.Create(
                    exception.StatusCode,
                    this.JsonResponse(ResponseStatus.UnknownError, exception));
            }
            catch (NotImplementedException exception)
            {
                return CommandResponse.Create(
                    HttpStatusCode.NotImplemented,
                    this.JsonResponse(ResponseStatus.UnknownCommand, exception));
            }
            catch (Exception exception)
            {
                return CommandResponse.Create(
                    HttpStatusCode.OK,
                    this.JsonResponse(ResponseStatus.UnknownError, exception));
            }
        }

        #endregion

        #region Methods

        protected virtual string DoImpl()
        {
            throw new InvalidOperationException("DoImpl should never be called in CommandExecutorBase");
        }

        protected string JsonResponse(ResponseStatus status = ResponseStatus.Success, object value = null)
        {
            return JsonConvert.SerializeObject(
                new JsonResponse(this.Automator.Session, status, value),
                Formatting.Indented);
        }

        #endregion
    }
}
