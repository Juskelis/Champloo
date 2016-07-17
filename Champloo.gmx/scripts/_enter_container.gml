/*
 * Internal script actually adding calling component
   to the container.
 */

//stop the procedure when container is already entered
if (contained_by == argument0)
    exit;

//leave the previous container properly
leave_container();

//add calling component to the new container
with (argument0) {
    script_execute(component_adder, other.id, argument1);
    layout_changed = true;
    }
contained_by = argument0;
