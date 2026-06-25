const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 3000;
const SCORES_FILE = path.join(__dirname, 'scores.json');
const MAX_ENTRIES = 5;
const PLACEHOLDER = { initials: '---', score: 0 };

app.use(cors());
app.use(bodyParser.json());

function readScores() {
    if (!fs.existsSync(SCORES_FILE)) {
        const initial = Array.from({ length: MAX_ENTRIES }, () => ({ ...PLACEHOLDER }));
        fs.writeFileSync(SCORES_FILE, JSON.stringify(initial, null, 2));
        return initial;
    }
    return JSON.parse(fs.readFileSync(SCORES_FILE, 'utf8'));
}

function writeScores(scores) {
    fs.writeFileSync(SCORES_FILE, JSON.stringify(scores, null, 2));
}

function sortDescending(scores) {
    return scores.slice().sort((a, b) => b.score - a.score);
}

app.get('/leaderboard', (req, res) => {
    const scores = readScores();
    res.json({ scores: sortDescending(scores) });
});

app.post('/leaderboard', (req, res) => {
    const { initials, score } = req.body;

    if (typeof initials !== 'string' || typeof score !== 'number') {
        return res.status(400).json({ error: 'initials (string) and score (number) are required' });
    }

    const scores = readScores();
    const real = scores.filter(e => e.initials !== PLACEHOLDER.initials);
    const hasRoom = real.length < MAX_ENTRIES;
    const lowestRealScore = real.length > 0 ? Math.min(...real.map(e => e.score)) : -1;

    if (!hasRoom && score <= lowestRealScore) {
        return res.json({ scores: sortDescending(scores) });
    }

    real.push({ initials, score });
    const top = sortDescending(real).slice(0, MAX_ENTRIES);

    while (top.length < MAX_ENTRIES) {
        top.push({ ...PLACEHOLDER });
    }

    writeScores(top);
    res.json({ scores: top });
});

app.listen(PORT, () => {
    console.log('Leaderboard server running on port ' + PORT);
});
