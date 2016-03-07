horizontal_amount = gamepad_axis_value(player_number, gp_axislh);
vertical_amount = gamepad_axis_value(player_number, gp_axislv);
aim_direction = point_direction(x,y, x+horizontal_amount*20,y+vertical_amount*20);

key_right = max(0, horizontal_amount);
key_left = min(horizontal_amount, 0);

key_jump = gamepad_button_check_pressed(player_number, gp_face1);
key_jump_released = gamepad_button_check_released(player_number, gp_face1);

key_block = gamepad_button_check(player_number, gp_shoulderlb)
                || gamepad_button_check(player_number, gp_shoulderl);

key_attack_normal = gamepad_button_check_pressed(player_number, gp_shoulderrb)
                || gamepad_button_check_pressed(player_number, gp_shoulderr);

key_shoot_press = gamepad_button_check_pressed(player_number, gp_face4);
key_shoot_release = gamepad_button_check_released(player_number, gp_face4);

key_dash = gamepad_button_check_pressed(player_number, gp_face3);

key_parry = !has_sword && (gamepad_button_check_pressed(player_number, gp_shoulderlb)
                            || gamepad_button_check_pressed(player_number, gp_shoulderl));

scr_filter_input();
