/*
    obj_toggle constructor( label, on_pressed_call, [on_pressed_args] )
*/

var _args = argument0;

if(ds_list_empty(_args))
{
    err("Need to have at least one argument!");
    exit;
}

label = ds_list_find_value(_args, 0);
height = max(height, string_height(label));
width = max(width, string_width(label) + height);

on_press = ds_list_find_value(_args, 1);
if(ds_list_size(_args) > 1) {
    for(var i = 2; i < ds_list_size(_args); i++) {
        on_press_args[i-2] = ds_list_find_value(_args, i);
    }
}