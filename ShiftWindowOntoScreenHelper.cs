﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace KSP_WPF
{
    public static class ShiftWindowOntoScreenHelper
    {
        /// <summary>
        ///     Intent:  
        ///     - Shift the window onto the visible screen.
        ///     - Shift the window away from overlapping the task bar.
        /// </summary>
        public static void ShiftWindowOntoScreen(Window window)
        {
            // Note that "window.BringIntoView()" does not work.                            
            if (window.Top < SystemParameters.VirtualScreenTop)
            {
                window.Top = SystemParameters.VirtualScreenTop;
            }

            if (window.Left < SystemParameters.VirtualScreenLeft)
            {
                window.Left = SystemParameters.VirtualScreenLeft;
            }

            if (window.Left + window.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
            {
                window.Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - window.Width;
            }

            if (window.Top + window.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
            {
                window.Top = SystemParameters.VirtualScreenHeight + SystemParameters.VirtualScreenTop - window.Height;
            }

            // Shift window away from taskbar.
            {
                var taskBarLocation = GetTaskBarLocationPerScreen();

                // If taskbar is set to "auto-hide", then this list will be empty, and we will do nothing.
                foreach (var taskBar in taskBarLocation)
                {
                    Rectangle windowRect = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);

                    // Keep on shifting the window out of the way.
                    int avoidInfiniteLoopCounter = 25;
                    while (windowRect.IntersectsWith(taskBar))
                    {
                        avoidInfiniteLoopCounter--;
                        if (avoidInfiniteLoopCounter == 0)
                        {
                            break;
                        }

                        // Our window is covering the task bar. Shift it away.
                        var intersection = Rectangle.Intersect(taskBar, windowRect);

                        if (intersection.Width < window.Width
                            // This next one is a rare corner case. Handles situation where taskbar is big enough to
                            // completely contain the status window.
                            || taskBar.Contains(windowRect))
                        {
                            if (taskBar.Left == 0)
                            {
                                // Task bar is on the left. Push away to the right.
                                window.Left = window.Left + intersection.Width;
                            }
                            else
                            {
                                // Task bar is on the right. Push away to the left.
                                window.Left = window.Left - intersection.Width;
                            }
                        }

                        if (intersection.Height < window.Height
                            // This next one is a rare corner case. Handles situation where taskbar is big enough to
                            // completely contain the status window.
                            || taskBar.Contains(windowRect))
                        {
                            if (taskBar.Top == 0)
                            {
                                // Task bar is on the top. Push down.
                                window.Top = window.Top + intersection.Height;
                            }
                            else
                            {
                                // Task bar is on the bottom. Push up.
                                window.Top = window.Top - intersection.Height;
                            }
                        }

                        windowRect = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Returned location of taskbar on a per-screen basis, as a rectangle. See:
        /// https://stackoverflow.com/questions/1264406/how-do-i-get-the-taskbars-position-and-size/36285367#36285367.
        /// </summary>
        /// <returns>A list of taskbar locations. If this list is empty, then the taskbar is set to "Auto Hide".</returns>
        private static List<Rectangle> GetTaskBarLocationPerScreen()
        {
            List<Rectangle> dockedRects = new List<Rectangle>();
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Equals(screen.WorkingArea) == true)
                {
                    // No taskbar on this screen.
                    continue;
                }

                Rectangle rect = new Rectangle();

                var leftDockedWidth = Math.Abs((Math.Abs(screen.Bounds.Left) - Math.Abs(screen.WorkingArea.Left)));
                var topDockedHeight = Math.Abs((Math.Abs(screen.Bounds.Top) - Math.Abs(screen.WorkingArea.Top)));
                var rightDockedWidth = ((screen.Bounds.Width - leftDockedWidth) - screen.WorkingArea.Width);
                var bottomDockedHeight = ((screen.Bounds.Height - topDockedHeight) - screen.WorkingArea.Height);
                if ((leftDockedWidth > 0))
                {
                    rect.X = screen.Bounds.Left;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = leftDockedWidth;
                    rect.Height = screen.Bounds.Height;
                }
                else if ((rightDockedWidth > 0))
                {
                    rect.X = screen.WorkingArea.Right;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = rightDockedWidth;
                    rect.Height = screen.Bounds.Height;
                }
                else if ((topDockedHeight > 0))
                {
                    rect.X = screen.WorkingArea.Left;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = screen.WorkingArea.Width;
                    rect.Height = topDockedHeight;
                }
                else if ((bottomDockedHeight > 0))
                {
                    rect.X = screen.WorkingArea.Left;
                    rect.Y = screen.WorkingArea.Bottom;
                    rect.Width = screen.WorkingArea.Width;
                    rect.Height = bottomDockedHeight;
                }
                else
                {
                    // Nothing found!
                }

                dockedRects.Add(rect);
            }

            if (dockedRects.Count == 0)
            {
                // Taskbar is set to "Auto-Hide".
            }

            return dockedRects;
        }
    }
}
