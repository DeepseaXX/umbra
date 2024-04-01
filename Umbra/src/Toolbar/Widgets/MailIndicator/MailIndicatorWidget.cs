﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Runtime.InteropServices;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MailIndicator;

[Service]
internal class MailIndicatorWidget : IToolbarWidget
{
    [ConfigVariable(
        "Toolbar.Widget.MailIndicator.Enabled",
        "Toolbar Widgets",
        "Show unread mail indicator",
        "Display a mail indicator when there are unread messages waiting for you."
    )]
    private static bool Enabled { get; set; } = true;

    public Element Element { get; } = new(
        id: "MailIndicatorWidget",
        size: new(28, 28),
        anchor: Anchor.MiddleRight,
        sortIndex: int.MinValue + 10,
        children: [
            new BackgroundElement(color: 0xFF1A1A1A, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4),
            new BorderElement(color: 0xFF3F3F3F, rounding: 3, padding: new(1)),
            new(
                id: "MailIcon",
                anchor: Anchor.None,
                text: FontAwesomeIcon.Envelope.ToIconString(),
                style: new() {
                    Font         = Font.FontAwesome,
                    TextAlign    = Anchor.MiddleCenter,
                    TextColor    = 0xFFC0C0C0,
                    OutlineColor = 0xFF000000,
                    OutlineWidth = 1,
                    TextOffset   = new(0, -1)
                }
            ),
        ]
    );

    public void OnDraw() { }

    public unsafe void OnUpdate()
    {
        Element.IsVisible = false;

        if (!Enabled) return;

        var infoModule = InfoModule.Instance();
        if (infoModule == null) return;

        var ipl = (InfoProxyLetterCount*)infoModule->GetInfoProxyById(InfoProxyId.Letter);
        if (ipl == null) return;

        Element.IsVisible = ipl->NumLetters > 0;
        Element.Tooltip = $"You have {ipl->NumLetters} new message{(ipl->NumLetters > 1 ? "s" : "")}";
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InfoProxyLetterCount
    {
        [FieldOffset(0x26)] public byte NumLetters;
    }
}