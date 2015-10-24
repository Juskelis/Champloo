///scr_furthest_spawn(player)

with(obj_Spawn)
{
    nearest = room_width*2;
    var i;
    var player;
    for(i = 0; i < instance_number(obj_Player); i += 1)
    {
        player = instance_find(obj_Player, i);
        if(point_distance(x,y, player.x,player.y) < nearest)
        {
            nearest = point_distance(x,y, player.x,player.y);
        }
    }
}

var furthest = noone;
for(var i = 0; i < instance_number(obj_Spawn); i += 1)
{
    if(furthest == noone || instance_find(obj_Spawn, i).nearest > furthest.nearest)
    {
        furthest = instance_find(obj_Spawn, i);
    }
}

return furthest;
