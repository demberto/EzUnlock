/*
 * SPDX-FileCopyrightText: 2022 demberto <demberto@protonmail.com>
 *
 * SPDX-License-Identifier: MIT
 */

using System.Diagnostics;
using System.Windows;

namespace UI
{

    public partial class App : Application
    {
        // https://stackoverflow.com/a/66477986/
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(
                typeof(System.Windows.Documents.Hyperlink),
                System.Windows.Documents.Hyperlink.RequestNavigateEvent,
                new System.Windows.Navigation.RequestNavigateEventHandler(
                    (sender, en) => Process.Start(new ProcessStartInfo(
                        en.Uri.ToString()
                    )
                    { UseShellExecute = true })
                )
            );
            base.OnStartup(e);
        }
    }
}
