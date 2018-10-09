SetTitleMatchMode, 3
#InstallKeybdHook
#UseHook
#InstallMouseHook
#MenuMaskKey, vkFF
#IfWinActive, Guild Wars 2 ahk_class ArenaNet_Dx_Window_Class
#NoEnv
#Persistent
onClipBoardChange("ClipChanged")


ClipChanged() {
	SetKeyDelay, 30, 30
	UniqueID := WinActive("ahk_class ArenaNet_Dx_Window_Class")
	if (RegExMatch(Clipboard, "\[&........\]") > 0) {
		
		if (UniqueID) {
			Send, {enter}^v{enter}
		}
		
		else {
			ControlSend, ,{enter}^v{enter}, Guild Wars 2 ahk_class ArenaNet_Dx_Window_Class
			WinActivate, Guild Wars 2 ahk_class ArenaNet_Dx_Window_Class
		}
	}
}
