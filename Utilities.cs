using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiPathTeam.StatusProgress.Activities
{
    public static class Utilities
    {

        #region Switches
        public static string LocPosition(string loc)
        {
            switch (loc)
            {
                case "lefttop":
                case "topleft":
                case "leftcenter":
                case "centerleft":
                case "leftbottom":
                case "bottomleft":
                case "centertop":
                case "topcenter":
                case "centerbottom":
                case "bottomcenter":
                case "righttop":
                case "topright":
                case "rightcenter":
                case "centerright":
                case "rightbottom":
                case "bottomright":
                    break;
                case "right":
                case "left":
                case "center":
                case "top":
                case "bottom":
                    loc += "center";
                    break;
                default:
                    loc = "centercenter";
                    break;
            }
            return loc;
        }

        public static Form LocLocation(string loc, Form frm, int ScreenHeight, int ScreenWidth, int offset = 0)
        {
            switch (loc)
            {
                case "lefttop":
                case "topleft":
                    frm.Location = new Point(0, 0);
                    break;
                case "leftcenter":
                case "centerleft":
                    frm.Location = new Point(0, (Convert.ToInt32(ScreenHeight / (double)2)) - frm.ClientSize.Height);
                    break;
                case "leftbottom":
                case "bottomleft":
                    frm.Location = new Point(0, ScreenHeight - frm.ClientSize.Height - 40);
                    break;
                case "centertop":
                case "topcenter":
                    frm.Location = new Point((System.Convert.ToInt32(ScreenWidth / (double)2)) - System.Convert.ToInt32(frm.ClientSize.Width / (double)2), 0);
                    break;
                case "centerbottom":
                case "bottomcenter":
                    frm.Location = new Point((System.Convert.ToInt32(ScreenWidth / (double)2)) - System.Convert.ToInt32(frm.ClientSize.Width / (double)2), ScreenHeight - frm.ClientSize.Height - 40);
                    break;
                case "righttop":
                case "topright":
                    frm.Location = new Point(ScreenWidth - frm.ClientSize.Width - offset, 0);
                    break;
                case "rightcenter":
                case "centerright":
                    frm.Location = new Point(ScreenWidth - frm.ClientSize.Width - offset, (System.Convert.ToInt32(ScreenHeight / (double)2)) - frm.ClientSize.Height);
                    break;
                case "rightbottom":
                case "bottomright":
                    frm.Location = new Point(ScreenWidth - frm.ClientSize.Width - offset, ScreenHeight - frm.ClientSize.Height - 40);
                    break;
                default:
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    break;
            }
            return frm;
        }

        #endregion

    }
}