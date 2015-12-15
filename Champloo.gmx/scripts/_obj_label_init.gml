/*
    obj_label constructor ( label, font, color )
*/

var _args;
_args = argument0;
label = ds_list_find_value(_args, 0);
font = ds_list_find_value(_args, 1);
color = ds_list_find_value(_args, 2);

draw_set_font(font);
width = string_width(label) + 20;
height = string_height(label) + 6;
