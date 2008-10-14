#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              TabControl
#              TabPage
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "TabControl" and "TabPage"  control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Label, Form, TabPage, TabControl, StatusBar, StatusBarPanel, 
    StatusBarPanelBorderStyle, StatusBarPanelAutoSize, Button, CheckBox, 
    RadioButton, TextBox
)
from System.Drawing import Point

class TabControlTabPageSample(Form):
    """TabControlTabPage control class"""

    def __init__(self):
        """TabControlTabPageSample class init function."""

        # setup title
        self.Text = "TabControl & TabPage control"

        # add items
        self.label1 = Label()
        self.label1.Text = "I'm in tab page #1"
        self.label1.Location = Point(5, 20)

        self.label2 = Label()
        self.label2.Text = "I'm in tab page #2"
        self.label2.Location = Point(5, 20)

        self.label3 = Label()
        self.label3.Text = "I'm in tab page #3"
        self.label3.Location = Point(5, 20)

        self.label4 = Label()
        self.label4.Text = "I'm in tab page #4"
        self.label4.Location = Point(5, 20)

        self.diclabels = {0: self.label1, 
                         1: self.label2,
                         2: self.label3,
                         3: self.label4,
                         }

        self.button = Button()
        self.button.Text = "Button"
        self.button.Location = Point(5, 60)
        
        self.textbox = TextBox()
        self.textbox.Text = "TextBox"
        self.textbox.Location = Point(5, 60)
        
        self.checkbox = CheckBox()
        self.checkbox.Text = "CheckBox"
        self.checkbox.Location = Point(5, 60)
        
        self.radiobutton = RadioButton()
        self.radiobutton.Text = "RadioButton"
        self.radiobutton.Location = Point(5, 60)
        
        self.dicitems = {0: self.button,
                         1: self.textbox,                        
                         2: self.checkbox,                        
                         3: self.radiobutton                        
                         }

        # setup tabcontrol
        self.tabcontrol = TabControl()
        self.tabcontrol.Width = 260
        self.tabcontrol.Height = 240

        # setup tabpage
        for i in range(4):
            self.tabpage = TabPage()
            self.tabpage.Text = "Tab %s" % i
            self.tabpage.Enter += self.on_click
            self.tabpage.Controls.Add(self.diclabels[i])
            self.tabpage.Controls.Add(self.dicitems[i])

            # add controls
            self.tabcontrol.TabPages.Add(self.tabpage)

        # setup status bar
        self.statusbar = StatusBar()
        self.statusbar_panel = StatusBarPanel()
        self.statusbar_panel.BorderStyle = StatusBarPanelBorderStyle.Sunken
        self.statusbar_panel.Text = "Select a Tab" 
        self.statusbar_panel.AutoSize = StatusBarPanelAutoSize.Spring
        self.statusbar.ShowPanels = True

        # add controls
        self.statusbar.Panels.Add(self.statusbar_panel)
        self.Controls.Add(self.statusbar)
        self.Controls.Add(self.tabcontrol)

    def on_click(self, sender, event):
        self.statusbar_panel.Text = "The current tab is: " + sender.Text

# run application
form = TabControlTabPageSample()
Application.EnableVisualStyles()
Application.Run(form)
