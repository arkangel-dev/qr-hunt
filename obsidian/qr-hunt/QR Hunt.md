
## Basic Features

- Players shall be able to scan QR codes which will increment their score
- Moderators shall be able to add QR codes to a game
- Moderators shall be able to create and modify existing games
- Administrators shall be able to edit moderators
- Players shall be able to register themselves using either a username/password combination or using a unique identifier


# Backend
## Endpoints

- Players shall be able to scan QR codes which will increment their score as well as fetch their score:
	- `/Api/{GAME_ID}/Scan?Code={ENTER-STRING-CODE-HERE}`
	- `/Api/{GAME_ID}/Verify?Code={ENTER-STRING-CODE-HERE}`
	- `/Api/{GAME_ID}/GetLeaderboard`

- Moderators shall be able to add QR codes to a game
	- `/Api/Games/{GAME_ID}/QRCode/Upload`
	- `/Api/Games/{GAME_ID}/QRCode/UploadBulk`
	- `/Api/Games/{GAME_ID}/QRCode/Delete/{QrId}`
	- `/Api/Games/{GAME_ID}/QRCode/GetQrList`

- Moderators shall be able to create and modify existing games
	- `/Api/Games/Create`
	- `/Api/Games/Modify`
	- `/Api/Games/Delete`

- Administrators shall be able to edit moderators
	- `/Api/Admin/SetRoles`

- Players shall be able to register themselves using either a username/password combination or using a unique identifier
	- `/Api/Login`
	- `/Api/Register`

## Database 

![[QrHunt.drawio]]
![[Pasted image 20241113174722.png]]
