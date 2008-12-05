#!/usr/bin/env python

####
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: Test accessibility of combobox_dropdown widget 
#              Use the comboboxdropdownframe.py wrapper script
#              Test the samples/combobox_dropdown.py script
####

# The docstring below  is used in the generated log file
"""
Test accessibility of combobox_dropdown widget
"""

# imports
import sys
import os

from strongwind import *
from combobox_dropdown import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the combobox_dropdown sample application
try:
  app = launchComboBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbddFrame = app.comboBoxDropDownFrame

#check ComboBox's actions
actionsCheck(cbddFrame.combobox, "ComboBox")

#do press action to show menu item list
cbddFrame.press(cbddFrame.combobox)

#check ComboBox item's actions list
actionsCheck(cbddFrame.menu, "Menu")
actionsCheck(cbddFrame.menuitem[0], "MenuItem")

#check default states of ComboBox, menu and text
statesCheck(cbddFrame.combobox, "ComboBox")
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selected"])
###print "text states list:" , cbddFrame.textbox._accessible.getState().getStates()

#check menuitem0,1's default states
statesCheck(cbddFrame.menuitem[0], "MenuItem")
###print "menuitem0 states list:" , cbddFrame.menuitem[0]._accessible.getState().getStates()

statesCheck(cbddFrame.menuitem[1], "MenuItem", \
                                add_states=["focused", "selected"])
###print "menuitem1 states list:" , cbddFrame.menuitem[1]._accessible.getState().getStates()

#check menuitem's text is implemented
cbddFrame.assertItemText()

#keyCombo down to select menuitem2
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 2
cbddFrame.assertLabel('2')
#menuitem2 up focused and selected states
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem1 get rid of visible and showing states
statesCheck(cbddFrame.menuitem[1], "MenuItem", invalid_states=["visible", "showing"])

#keyCombo down to select menuitem3
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 3
cbddFrame.assertLabel('3')
#menuitem3 up focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem2 get rid of visible and showing states
statesCheck(cbddFrame.menuitem[2], "MenuItem", invalid_states=["visible", "showing"])

#keyCombo up to select menuitem2
cbddFrame.keyCombo("Up", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 2
cbddFrame.assertLabel('2')
#menuitem2 up focused and selected states
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem3 get rid of focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem")

#do click action to select menuitem0 to update text value and label
cbddFrame.click(cbddFrame.menuitem[0])
sleep(config.SHORT_DELAY)
#change label's text to You select 0
cbddFrame.assertLabel('0')
#change textbox value to 0
cbddFrame.assertText(cbddFrame.textbox, 0)
#menuitem0 up selected state
statesCheck(cbddFrame.menuitem[0], "MenuItem", add_states=["selected"])

#do click action to select menuitem9 to update text value and label
cbddFrame.click(cbddFrame.menuitem[9])
sleep(config.SHORT_DELAY)
#change label's text to You select 9
cbddFrame.assertLabel('9')
#change textbox value to 9
cbddFrame.assertText(cbddFrame.textbox, 9)
#menuitem9 up selected state
statesCheck(cbddFrame.menuitem[9], "MenuItem", add_states=["selected"])
#menuitem0 get rid of selected, visible, showing states
statesCheck(cbddFrame.menuitem[0], "MenuItem", invalid_states=["visible", "showing"])

#enter value to textbox
#inter '6' to text box to check the text value
cbddFrame.inputText(cbddFrame.textbox, "6")
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel("6")
#the text of textbox is changed to 6
cbddFrame.assertText(cbddFrame.textbox, "6")
#menuitem6 would be selected
statesCheck(cbddFrame.menuitem[6], "MenuItem")

#test editable Text by enter text value '8' 
cbddFrame.enterTextValue(cbddFrame.textbox,"8")
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel(8)
#the text of textbox is changed to 8
cbddFrame.assertText(cbddFrame.textbox, 8)
#menuitem8 would be selected
statesCheck(cbddFrame.menuitem[8], "MenuItem")

#check combo box selection is implemented
#select menu to rise selected
cbddFrame.assertSelectionChild(cbddFrame.combobox, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu", add_states=["selected"])
statesCheck(cbddFrame.textbox, "Text")
#select text to rise selected
cbddFrame.assertSelectionChild(cbddFrame.combobox, 1)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text", add_states=["selected"])
#clear selection to get rid of selected
cbddFrame.assertClearSelection(cbddFrame.combobox)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text")

#check menu selection is implemented
#select item3 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 3)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[3], "MenuItem", add_states=["focused", "selected"])
#select item5 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
#item3 get rid of focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem")

#clear selection
cbddFrame.assertClearSelection(cbddFrame.menu)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem")

#close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
