"use strict";define(["app"],function(a){a.run(["$templateCache",function(b){b.put("nggrid/headerCellTemplateDate.html",'<div class="ngHeaderSortColumn {{col.headerClass}}" ng-style="{cursor: col.cursor}" ng-class="{ ngSorted: !noSortVisible }"><div ng-click="col.sort($event)" ng-class="\'colt\' + col.index" class="ngHeaderText"><i class=\'fa fa-calendar text-muted\'></i>&nbsp;&nbsp;{{col.displayName}}</div><div class="ngSortButtonDown" ng-show="col.showSortButtonDown()"></div><div class="ngSortButtonUp" ng-show="col.showSortButtonUp()"></div><div class="ngSortPriority">{{col.sortPriority}}</div></div><div ng-if="col.resizable" class="ngHeaderGrip" ng-click="col.gripClick($event)" ng-mousedown="col.gripOnMouseDown($event)"></div>');b.put("nggrid/headerCellTemplateTime.html",'<div class="ngHeaderSortColumn {{col.headerClass}}" ng-style="{cursor: col.cursor}" ng-class="{ ngSorted: !noSortVisible }"><div ng-click="col.sort($event)" ng-class="\'colt\' + col.index" class="ngHeaderText"><i class=\'fa fa-clock-o text-muted\'></i>&nbsp;&nbsp;{{col.displayName}}</div><div class="ngSortButtonDown" ng-show="col.showSortButtonDown()"></div><div class="ngSortButtonUp" ng-show="col.showSortButtonUp()"></div><div class="ngSortPriority">{{col.sortPriority}}</div></div><div ng-if="col.resizable" class="ngHeaderGrip" ng-click="col.gripClick($event)" ng-mousedown="col.gripOnMouseDown($event)"></div>')}])});