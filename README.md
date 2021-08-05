# CMS-plugin-Optimizely


When changing in files in ClientResources then update the version number foldername of the folder ClientResources is in. 
for example from 1.0.1 to 1.0.2 and in module.config update clientResourceRelativePath to the same new number

when any change have been made update the package number in the project file
from for example <Version>4.0.7</Version> to <Version>4.0.8</Version>


updating module name:
change in Constants.SiteImproveModuleName and in all js files


Generate local nuget feed:

create folder to host the feed for example /c/Projects/SiteimprovePlugin/nuget-feed

PS .\nuget.exe add SiteImprove.Optimizely.Plugin\bin\Debug\SiteImprove.Optimizely.Plugin.4.0.6-rc.nupkg -Source nuget-feed
$ ./nuget.exe add SiteImprove.Optimizely.Plugin/bin/Debug/SiteImprove.Optimizely.Plugin.4.0.6-rc.nupkg -Source /c/Projects/SiteimprovePlugin/nuget-feed
