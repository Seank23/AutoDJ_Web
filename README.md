# AutoDJ
URL: https://autodj.azurewebsites.net/

![AutoDJ](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/AutoDJ1.PNG)

## What is it?
AutoDJ is a web app that puts the power of queueing music into the hands of all listeners. Having a party? Playing online with friends? Use AutoDJ to create the perfect music queue for any situation: by allowing everyone to participate. Simply create a session and invite friends to join. Any user can add a song to the queue by using the search bar and selecting a result. Once added to the queue, other users can 'Vote' for a song to increase its rating and importantly, its position in the queue.

## How does it work?
AutoDJ uses YouTube for music streaming and makes use of YouTube's Data and IFrame APIs for searching and playing videos respectively. The app heavily utilises the SignalR library  to provide real-time, two-way communication between clients and the server to allow all clients in a given session to be in sync with eachother. Users can choose to use the app in 'offline' mode by not creating/joining a session, in this mode the queue is stored client-side on the user's device and only they can access it. Sessions are stored server-side allowing users on different devices to connect and access the same queue. Sessions are assigned 6-digit keys called Session IDs, these denote individual sessions and are what allow other users to connect to the session.

## Features
- Intuitive and fully responsive UI, works on any device.
- YouTube search and playback.
- Queue entire YouTube playlists.
- Control the order of the queue by voting.
- Create and join private music sessions.
- Setup session permissions to allow/forbid certain features for other users.

## Current Limitations
- Maximum of ~100 TOTAL searches per day due to API quota limit.
- Maximum of 20 TOTAL simultaneous connections.
- No queue size limit however larger queues my cause considerable lag.

## Guide
### Creating A Session
To create a session, click on the 'Create Session' button in the navbar. You will be presented with a confirmation popup window, here you will also be able to enable or disable certain functionality for other users.<br/><br/>
![CreateSession](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/CreateSession.PNG)<br/><br/>
Once a session has been successfully created, you will be given a Session ID. This is a six-digit code which identifies your session and allows others to connect to it. Your Session ID is displayed in the navbar for easy access, clicking on this will present a popup window also displaying the SessionID but in a copyable form.<br/><br/>
![SessionCreated](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/SessionCreated.PNG)

### Joining A Session
To join a session, click on the 'Join Session' button in the navbar. You will be presented with a popup window containing a field to enter a Session ID. Simply enter the Session ID provided by the session's creator, if successful your queue will be synced with the session.

### Adding A Song
When you want to add a song to the queue, enter the name of the song or other relevant search criteria into the Add Song search bar, making sure the 'Video' toggle is selected and click the Search button. A request will be sent to YouTube's searching API which may take a few moments to process. Once the request has been processed you will be shown upto 5 video results to choose from, with details about the video and its thumbnail being displayed. Once you choose a video, click the 'Add To Queue' button and the video will be placed on the queue with one vote.<br/><br/>
![AddSong](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/AddSong.PNG)

### Adding A Playlist
** This feature may be disabled for session users ** <br/><br/>
Similarly to adding a song, a YouTube playlist can be added to the queue by entering keywords into the Add Song search bar and selecting 'Playlist' from the the toggle. You will be presented with upto 5 relevant YouTube playlists with accompanying details to choose from. Once a playlist is added to the queue, the app will retrive all videos and add them to queue individually, this may take some time especially for large playlists. Videos added from a playlist will have no votes by default.<br/><br/>
![AddPlaylist](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/AddPlaylist.PNG)

### The Queue
Arguably the most importent component of AutoDJ, the queue lists all of the songs that have been requested in a session in the order that they will be played. When a user wants to add a song to the queue, a queue item containing the songs details will be created and appended to the queue. A queue item consists of the title and duration of the video and channel it belongs to, along with its thumbnail and two buttons to vote for the song and remove it (may be disabled) respectively.<br/><br/>
![QueueItem](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/QueueItem.PNG)<br/><br/>
The voting system allows any user connected to the session to vote for songs they would like to play, the queue is ordered based on the number of votes each song has so the most popular songs will rise to the top of the queue and subsequently be the next to be played.<br/><br/>
![QueueVotes](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/QueueVotes.PNG)

### Playback Controls
The playback controls consist of a  play/pause button, a stop button and a skip button as well as a volume slider. When connected to a session the pause, stop and skip buttons affect the entire session while the volume slider will only change the volume for that device. The stop and skip buttons may be disabled for session users.<br/><br/>
![Controls](https://github.com/Seank23/AutoDJ_Web/blob/master/Images/Controls.PNG)
