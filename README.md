# HiLo Game API
## Introduction
This API implements the HiLo game, allowing clients to create players, start a new game, restart an existing game, take a guess, retrieve a specific game or player, and retrieve all games and players.

## Getting started
These instructions will help you set up a local instance of the API.

### Prerequisites
- Docker

### Installing
1. Clone the repository to your local machine.
``` bash
git clone https://github.com/JoaoBCoelho/HiLoGame.git
```

2. Navigate to the cloned repository folder.
``` bash
cd HiLoAPI
```

3. Create the required images, create the network and run the containers
``` bash
docker-compose up
```

4. You can access the application on http://localhost:8080/swagger/index.html

## API Endpoints

The API has the following endpoints:

### Game
- **POST /api/games**: Start a new game.
- **POST /api/games/{id}/restart**: Restart a finished game.
- **POST /api/games/{id}/guess**: Take a guess in an existing game.
- **GET /api/games/{id}**: Retrieve information about a specific game.
- **GET /api/games**: Retrieve information about all games.

### Players
- **POST /api/players**: Create a new player.
- **GET /api/players/{id}**: Retrieve information about a specific player.
- **GET /api/players**: Retrieve information about all players.

## Request & Response Format
- ### **POST /api/games**

**Request**

Content-Type: application/json
```json
{
  "minValue": 1,
  "maxValue": 10,
  "players": [
    1, 2
  ]
}
```

**Response**

Content-Type: application/json
```json
{
  "minValue": 1,
  "maxValue": 10,
  "id": 1,
  "players": [
    {
      "id": 1,
      "name": "player 1"
    },
    {
      "id": 2,
      "name": "player 2"
    }
  ]
}
```
- ### **GET /api/games**

**Request**

No parameters

**Response**

Content-Type: application/json
```json
[
  {
    "minValue": 1,
    "maxValue": 10,
    "id": 1,
    "round": 1,
    "status": "Ongoing",
    "gamePlayerInfos": [
      {
        "gameId": 1,
        "playerId": 1,
        "playerName": "player 1",
        "attempts": 0,
        "winner": false
      },
      {
        "gameId": 1,
        "playerId": 2,
        "playerName": "player 2",
        "attempts": 0,
        "winner": false
      }
    ]
  }
]
```
- ### **POST /api/games/{id}/restart**

**Request**

Content-Type: path
```
/api/games/1/restart
```

**Response**

Content-Type: application/json
```json
{
  "minValue": 1,
  "maxValue": 10,
  "id": 2,
  "players": [
    {
      "id": 1,
      "name": "player 1"
    },
    {
      "id": 2,
      "name": "player 2"
    }
  ]
}
```
- ### **POST /api/games/{id}/guess**

**Request**

Content-Type: path
```
/api/games/1/guess
```

Content-Type: application/json
```json
{
  "playerId": 1,
  "guess": 5
}
```

**Response**

Content-Type: application/json
```json
{
  "gameId": 1,
  "playerId": 1,
  "playerName": "player 1",
  "attempts": 1,
  "result": "LO",
  "gameStatus": "Ongoing"
}
```
- ### **GET /api/games/{id}**

**Request**

Content-Type: path
```
/api/games/1
```

**Response**

Content-Type: application/json
```json
{
  "id": 1,
  "round": 1,
  "status": "Ongoing",
  "gamePlayerInfos": [
    {
      "gameId": 1,
      "playerId": 1,
      "playerName": "player 1",
      "attempts": 1,
      "winner": false
    },
    {
      "gameId": 1,
      "playerId": 2,
      "playerName": "player 2",
      "attempts": 0,
      "winner": false
    }
  ],
  "minValue": 1,
  "maxValue": 10
}
```
- ### **POST /api/players**

**Request**

Content-Type: application/json
```json
"player 1"
```

**Response**

Content-Type: application/json
```json
{
  "id": 1,
  "name": "player 1",
  "gamesPlayed": 0,
  "wins": 0
}
```
- ### **GET /api/players**

**Request**

No parameters

**Response**

Content-Type: application/json
```json
[
  {
    "id": 1,
    "name": "player 1",
    "gamesPlayed": 0,
    "wins": 0
  },
  {
    "id": 2,
    "name": "player 2",
    "gamesPlayed": 0,
    "wins": 0
  },
]
```
- ### **GET /api/players/{id}**

**Request**

Content-Type: path
```
/api/players/1
```

**Response**

Content-Type: application/json
```json
{
  "id": 1,
  "name": "player 1",
  "gamesPlayed": 0,
  "wins": 0
}
```

## Game Sequence
Here is an example of a game sequence.

![image](https://user-images.githubusercontent.com/32825344/217408108-9e7c2b36-af2a-4f9c-929c-9e5fed33a27c.png)


## Built with
.NET 6

## Author
[João Borges Coelho](https://github.com/JoaoBCoelho)
