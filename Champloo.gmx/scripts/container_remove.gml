///container_remove(container,component)
/*
 * Removes given component from a container, unless
   it's not in the container at all!
 * Removal is performed using "remove" method of
   the container.
 */

if (argument1.contained_by != argument0)
    exit;

with (argument1) { leave_container(); }