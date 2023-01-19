using System;
using System.Activities;
using System.ComponentModel;
using System.Windows.Forms;

namespace UiPathTeam.StatusProgress.Activities
{
    [DisplayName("Close Status"), Description("Closes the status form")]
    public class CloseStatus : NativeActivity
    {

        #region Properties

        [Category("Input"), Description("The Status Container to Destroy")]
        [RequiredArgument]
        public InArgument<Form> Container { get; set; }

        #endregion

        #region CodeActivity

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                Container.Get(context).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Status Container Closure Failed" + Environment.NewLine + ex.Message);
            }
            try
            {
                Container.Get(context).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Status Container Disposal Failed" + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region CheckProperties

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            if (Container == null)
            {
                metadata.AddValidationError($"Container must be set before CloseStatus activity '{DisplayName}' can be used.");
            }
        }

        #endregion

    }
}