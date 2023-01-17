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
        public InArgument<Form> Container { get; set; } = new Form();

        #endregion

        #region CodeActivity

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                Container.Get(context).Close();
                Container.Get(context).Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Status Container Closure Failed" + Environment.NewLine + ex.Message);
            }
        }

        #endregion

    }
}