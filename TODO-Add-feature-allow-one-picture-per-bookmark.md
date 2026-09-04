## FEATURE: Allow one picture per bookmark

 The app should support to optionally add one picture per bookmark. Adding the picture should be possible with paste using `<Ctrl + v>` or per `Drag'n drop`.
 
 On adding the picture is copied into a cloud synced share which location can be set by the user. The added picture is renamed to a `SHA-256 hash` (64 characters) according to its title and then copied to the cloud.
 
 When a bookmark is selected, the cloud storage is searched for a matching image and, if found, it is loaded. 

 The picture is displayed by injecting an  `![alt text](figures/picture.jpg)` into the Markdown before rendering. If the Markdown contains a quote `>` introduction block followed by an italic  *Author, date* block, it is injected thereafter. If the Markdown contains only quote `>` introduction block but no author date block, it is injected after the quote block. In all other cases the Markdown is injected at the very top.

 *Uwe Rosner, 04.09.2026*


 ### Steps

 - [ ] Add picture location to settings dialog
 - [ ] Add receiving a picture at `<Ctrl + v>` or `Drag'n drop`
 - [ ] Add saving the picture at storage location (with overwrite warning)
 - [ ] Add matching the picture on bookmark selection
 - [ ] Add injecting the picture to the markdown (with regex for the inject position)
