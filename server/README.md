# NES Tetris Leaderboard Server

Node.js Express server that persists the global top 5 scores to a JSON file.

## Setup

```
cd server
npm install
node index.js
```

Server listens on port 3000.

## Endpoints

### GET /leaderboard

Returns the current top 5 scores sorted descending by score.

```
GET http://localhost:3000/leaderboard
```

Response:
```json
{
  "scores": [
    {"initials": "AAA", "score": 12345},
    {"initials": "---", "score": 0}
  ]
}
```

### POST /leaderboard

Submit a new score. The entry is inserted only if it qualifies for the top 5.

```
POST http://localhost:3000/leaderboard
Content-Type: application/json

{"initials": "AAA", "score": 12345}
```

Response: updated `{"scores": [...]}` sorted descending.

A score that does not beat the lowest existing entry is rejected and the list is returned unchanged.

## Data File

Scores are stored in `server/scores.json`. The file is created automatically on first start with 5 placeholder entries.
