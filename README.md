# AutoDJ
https://autodj.azurewebsites.net/

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
