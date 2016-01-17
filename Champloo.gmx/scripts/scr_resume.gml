///scr_resume()
//  resumes the game
with(obj_Pause)
{
    event_perform(ev_other, ev_user1);
}

with(par_template)
{
    instance_destroy();
}
