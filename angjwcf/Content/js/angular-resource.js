/*
 AngularJS v1.3.0-rc.0
 (c) 2010-2014 Google, Inc. http://angularjs.org
 License: MIT
*/
(function(H,d,A){'use strict';function C(g,q){q=q||{};d.forEach(q,function(d,h){delete q[h]});for(var h in g)!g.hasOwnProperty(h)||"$"===h.charAt(0)&&"$"===h.charAt(1)||(q[h]=g[h]);return q}var w=d.$$minErr("$resource"),B=/^(\.[a-zA-Z_$][0-9a-zA-Z_$]*)+$/;d.module("ngResource",["ng"]).provider("$resource",function(){var g=this;this.defaults={stripTrailingSlashes:!0,actions:{get:{method:"GET"},save:{method:"POST"},query:{method:"GET",isArray:!0},remove:{method:"DELETE"},"delete":{method:"DELETE"}}};
this.$get=["$http","$q",function(q,h){function t(d,k){this.template=d;this.defaults=s({},g.defaults,k);this.urlParams={}}function v(x,k,l,m){function f(b,c){var f={};c=s({},k,c);r(c,function(a,c){u(a)&&(a=a());var d;if(a&&a.charAt&&"@"==a.charAt(0)){d=b;var e=a.substr(1);if(null==e||""===e||"hasOwnProperty"===e||!B.test("."+e))throw w("badmember",e);for(var e=e.split("."),n=0,k=e.length;n<k&&d!==A;n++){var h=e[n];d=null!==d?d[h]:A}}else d=a;f[c]=d});return f}function E(b){return b.resource}function e(b){C(b||
{},this)}var F=new t(x,m);l=s({},g.defaults.actions,l);e.prototype.toJSON=function(){var b=s({},this);delete b.$promise;delete b.$resolved;return b};r(l,function(b,c){var k=/^(POST|PUT|PATCH)$/i.test(b.method);e[c]=function(a,c,m,x){var n={},g,l,y;switch(arguments.length){case 4:y=x,l=m;case 3:case 2:if(u(c)){if(u(a)){l=a;y=c;break}l=c;y=m}else{n=a;g=c;l=m;break}case 1:u(a)?l=a:k?g=a:n=a;break;case 0:break;default:throw w("badargs",arguments.length);}var t=this instanceof e,p=t?g:b.isArray?[]:new e(g),
z={},v=b.interceptor&&b.interceptor.response||E,B=b.interceptor&&b.interceptor.responseError||A;r(b,function(b,a){"params"!=a&&"isArray"!=a&&"interceptor"!=a&&(z[a]=G(b))});k&&(z.data=g);F.setUrlParams(z,s({},f(g,b.params||{}),n),b.url);n=q(z).then(function(a){var c=a.data,f=p.$promise;if(c){if(d.isArray(c)!==!!b.isArray)throw w("badcfg",b.isArray?"array":"object",d.isArray(c)?"array":"object");b.isArray?(p.length=0,r(c,function(a){"object"===typeof a?p.push(new e(a)):p.push(a)})):(C(c,p),p.$promise=
f)}p.$resolved=!0;a.resource=p;return a},function(a){p.$resolved=!0;(y||D)(a);return h.reject(a)});n=n.then(function(a){var b=v(a);(l||D)(b,a.headers);return b},B);return t?n:(p.$promise=n,p.$resolved=!1,p)};e.prototype["$"+c]=function(a,b,d){u(a)&&(d=b,b=a,a={});a=e[c].call(this,a,this,b,d);return a.$promise||a}});e.bind=function(b){return v(x,s({},k,b),l)};return e}var D=d.noop,r=d.forEach,s=d.extend,G=d.copy,u=d.isFunction;t.prototype={setUrlParams:function(g,k,l){var m=this,f=l||m.template,h,
e,q=m.urlParams={};r(f.split(/\W/),function(b){if("hasOwnProperty"===b)throw w("badname");!/^\d+$/.test(b)&&b&&(new RegExp("(^|[^\\\\]):"+b+"(\\W|$)")).test(f)&&(q[b]=!0)});f=f.replace(/\\:/g,":");k=k||{};r(m.urlParams,function(b,c){h=k.hasOwnProperty(c)?k[c]:m.defaults[c];d.isDefined(h)&&null!==h?(e=encodeURIComponent(h).replace(/%40/gi,"@").replace(/%3A/gi,":").replace(/%24/g,"$").replace(/%2C/gi,",").replace(/%20/g,"%20").replace(/%26/gi,"&").replace(/%3D/gi,"=").replace(/%2B/gi,"+"),f=f.replace(new RegExp(":"+
c+"(\\W|$)","g"),function(b,a){return e+a})):f=f.replace(new RegExp("(/?):"+c+"(\\W|$)","g"),function(b,a,c){return"/"==c.charAt(0)?c:a+c})});m.defaults.stripTrailingSlashes&&(f=f.replace(/\/+$/,"")||"/");f=f.replace(/\/\.(?=\w+($|\?))/,".");g.url=f.replace(/\/\\\./,"/.");r(k,function(b,c){m.urlParams[c]||(g.params=g.params||{},g.params[c]=b)})}};return v}]})})(window,window.angular);
//# sourceMappingURL=angular-resource.min.js.map