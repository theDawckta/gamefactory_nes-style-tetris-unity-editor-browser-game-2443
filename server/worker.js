export default {
  async fetch(request, env) {
    const url = new URL(request.url);
    const cors = { 'Access-Control-Allow-Origin': '*', 'Content-Type': 'application/json' };
    const preflightCors = { 'Access-Control-Allow-Origin': '*', 'Access-Control-Allow-Methods': 'GET, POST, OPTIONS', 'Access-Control-Allow-Headers': 'Content-Type' };
    if (request.method === 'OPTIONS') return new Response(null, { headers: preflightCors });

    if (url.pathname === '/leaderboard' && request.method === 'GET') {
      const scores = JSON.parse(await env.SCORES.get('scores') || '[]');
      return new Response(JSON.stringify({ scores }), { headers: cors });
    }

    if (url.pathname === '/leaderboard' && request.method === 'POST') {
      const { initials, score } = await request.json();
      const scores = JSON.parse(await env.SCORES.get('scores') || '[]');
      scores.push({ initials: String(initials).slice(0, 3).toUpperCase(), score: parseInt(score) });
      scores.sort((a, b) => b.score - a.score);
      const top = scores.slice(0, 10);
      await env.SCORES.put('scores', JSON.stringify(top));
      return new Response(JSON.stringify({ scores: top }), { headers: cors });
    }

    return new Response('Not found', { status: 404 });
  }
};
