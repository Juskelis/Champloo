if(key_jump_pressed)
{
    press_time = get_timer();
}

if(key_jump)    //reset state
{
    press_time = -1;
    jumpstate = JumpStates.Normal;
}

if(press_time > 0 && get_timer() - press_time > hold_delay*1000000)
{
    jumpstate = JumpStates.Held;
}
