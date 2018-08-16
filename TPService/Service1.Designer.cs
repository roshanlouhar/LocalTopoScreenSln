namespace TPService
{
    partial class Service1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Required Timer to repeat event.
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "Service1";

            //this.timer = new System.Timers.Timer(interval * 1000);  // 30000 milliseconds = 30 seconds
            //this.timer.AutoReset = true;
            //this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            //this.timer.Start();
        }

        #endregion
    }
}
