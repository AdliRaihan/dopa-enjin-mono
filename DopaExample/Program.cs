
using DopaEngine;
using DopaExample;

DE _DE = DE.Get();
_DE.VM.SetActivity(new DETileMapEditor());
using var game = _DE;
game.Run();
