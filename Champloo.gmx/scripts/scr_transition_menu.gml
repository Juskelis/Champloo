///scr_transition_menu( [old_id, new] )
/*
    for this to work properly, do not use the keyword "self" for the old menu
        bad things will happen.
        Use id instead.
*/
var _args = argument0;
instance_create(0,0, _args[1]);
with(_args[0]) { instance_destroy(); }