///scr_spawn_player(player_number)

var furthest_spawn = scr_furthest_spawn_all();

var instance = instance_create(furthest_spawn.x, furthest_spawn.y, obj_Player);

with(instance)
{
    player_number = argument0;
}
