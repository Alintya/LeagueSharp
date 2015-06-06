#region

using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D;
using Color = SharpDX.Color;

#endregion

namespace Marksman.Utils
{
    internal static class Utils
    {
        private static readonly string[] BetterWithEvade =
        {
            "Corki", "Ezreal", "Graves", "Lucian", "Sivir", "Tristana",
            "Caitlyn", "Vayne"
        };

        public static void PrintMessage(string message)
        {
            Notifications.AddNotification("Marksman: " + message, 4000);
        }

        public static void DrawText(Font vFont, String vText, int vPosX, int vPosY, Color vColor)
        {
            vFont.DrawText(null, vText, vPosX + 2, vPosY + 2, vColor != Color.Black ? Color.Black : Color.White);
            vFont.DrawText(null, vText, vPosX, vPosY, vColor);
        }

        internal static class Jungle
        {
            public static void DrawJunglePosition()
            {
                if (Game.MapId == (GameMapId) 11)
                {
                    const float circleRange = 100f;

                    Render.Circle.DrawCircle(
                        new Vector3(7461.018f, 3253.575f, 52.57141f), circleRange, System.Drawing.Color.Blue);
                    // blue team :red
                    Render.Circle.DrawCircle(
                        new Vector3(3511.601f, 8745.617f, 52.57141f), circleRange, System.Drawing.Color.Blue);
                    // blue team :blue
                    Render.Circle.DrawCircle(
                        new Vector3(7462.053f, 2489.813f, 52.57141f), circleRange, System.Drawing.Color.Blue);
                    // blue team :golems
                    Render.Circle.DrawCircle(
                        new Vector3(3144.897f, 7106.449f, 51.89026f), circleRange, System.Drawing.Color.Blue);
                    // blue team :wolfs
                    Render.Circle.DrawCircle(
                        new Vector3(7770.341f, 5061.238f, 49.26587f), circleRange, System.Drawing.Color.Blue);
                    // blue team :wariaths

                    Render.Circle.DrawCircle(
                        new Vector3(10930.93f, 5405.83f, -68.72192f), circleRange, System.Drawing.Color.Yellow);
                    // Dragon

                    Render.Circle.DrawCircle(
                        new Vector3(7326.056f, 11643.01f, 50.21985f), circleRange, System.Drawing.Color.Red);
                    // red team :red
                    Render.Circle.DrawCircle(
                        new Vector3(11417.6f, 6216.028f, 51.00244f), circleRange, System.Drawing.Color.Red);
                    // red team :blue
                    Render.Circle.DrawCircle(
                        new Vector3(7368.408f, 12488.37f, 56.47668f), circleRange, System.Drawing.Color.Red);
                    // red team :golems
                    Render.Circle.DrawCircle(
                        new Vector3(10342.77f, 8896.083f, 51.72742f), circleRange, System.Drawing.Color.Red);
                    // red team :wolfs
                    Render.Circle.DrawCircle(
                        new Vector3(7001.741f, 9915.717f, 54.02466f), circleRange, System.Drawing.Color.Red);
                    // red team :wariaths                    
                }
            }
        }
    }
}