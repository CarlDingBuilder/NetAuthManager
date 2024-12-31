import{b as s}from"./basic-template.8d3ac169.js";import{_ as l,d as i,r as o,o as n,c as p,a as u,b as e,e as F,w as C,C as d,p as c,f as B}from"./index.17d3c514.js";const _=i({components:{basicTemplate:s},setup(){}}),E=a=>(c("data-v-1aa54704"),a=a(),B(),a),r={class:"layout-container"},v=E(()=>u("h1",null,"\u4F7F\u7528\u8BF4\u660E",-1)),D=E(()=>u("p",null,"\u4E3A\u4EC0\u4E48\u6587\u6863\u662F\u5199\u5728\u7CFB\u7EDF\u5185\uFF0C\u4E00\u65B9\u9762\u662F\u56E0\u4E3A\u5077\u61D2\uFF0C\u53E6\u4E00\u65B9\u9762\u662F\u56E0\u4E3A\u6211\u575A\u4FE1\uFF0C\u5982\u679C\u6211\u7528\u7740\u4E0D\u723D\uFF0C\u90A3\u4E48\u5927\u5BB6\u7528\u7740\u4E5F\u4E0D\u4F1A\u723D\uFF0C\u6240\u4EE5\u6587\u6863\u90E8\u5206\u5C31\u6682\u65F6\u653E\u7F6E\u4E8E\u6B64\u4E86",-1)),h={style:{padding:"0 10px"}},m=d("<h2 data-v-1aa54704>\u7B80\u4ECB</h2><p data-v-1aa54704>vue-admin-box\u662F\u4E00\u4E2A\u5F00\u6E90\u7684\u4E2D\u540E\u53F0\u7BA1\u7406\u9879\u76EE\uFF0C\u4E3B\u8981\u6280\u672F\u6808\u4E3Avue3\u5168\u5BB6\u6876\uFF0C\u5305\u62EC\u4E86\uFF1Avue3(vue-next)\u3001vite\u3001element-plus\u3001vue-router\u3001vuex\u3001axios\u3001vite-plugin-mock\u7B49\u4F17\u591A\u4E2D\u540E\u53F0\u9879\u76EE\u5E38\u7528\u6280\u672F\u3002</p><ol data-v-1aa54704><li data-v-1aa54704>vue3(vue-next)\uFF1A\u672C\u9879\u76EE\u7684\u6838\u5FC3\u6280\u672F\u6808\uFF0C\u7528\u4E8E\u54CD\u5E94\u5F0F\u6A21\u5757\u7684\u63A7\u5236</li><li data-v-1aa54704>vite\uFF1A\u672C\u5730\u670D\u52A1\u8FD0\u884C\u3001\u6253\u5305\u7F16\u8BD1\u5DE5\u5177\uFF0C\u7279\u70B9\uFF1A\u8F7B\u91CF\u3001\u5FEB\u901F</li><li data-v-1aa54704>element-plus\uFF1Avue3\u7684\u7EC4\u4EF6\u5E93</li><li data-v-1aa54704>vue-router\uFF1A\u8DEF\u7531\u63A7\u5236\u63D2\u4EF6\uFF0C\u672C\u9879\u76EE\u4E13\u95E8\u9488\u5BF9\u5B9E\u9645\u4E1A\u52A1\u4F5C\u4E86\u5927\u91CF\u9002\u914D</li><li data-v-1aa54704>vuex\uFF1A\u72B6\u6001\u7BA1\u7406\u5668\uFF0C\u5E76\u5199\u4E86\u4E00\u4E2A\u672C\u5730\u7F13\u5B58\u63D2\u4EF6\u4F9B\u4F7F\u7528</li><li data-v-1aa54704>axios\uFF1A\u6570\u636E\u8BF7\u6C42\u5E93\uFF0C\u57FA\u4E8E\u6B64\u505A\u4E86\u66F4\u8FDB\u4E00\u6B65\u7684\u5C01\u88C5</li><li data-v-1aa54704>vite-plugin-mock\uFF1A\u672C\u5730\u548C\u7EBF\u4E0Amock\u6570\u636E\u4E13\u7528\u5E93\uFF0C\u672C\u5730\u53EF\u67E5\u770B\u5230\u771F\u5B9Ehttp\u8BF7\u6C42\uFF0C\u7EBF\u4E0A\u4F7F\u7528js\u7248\u672C\u6765\u505A\u66FF\u6362</li></ol><h2 data-v-1aa54704>\u5173\u4E8Ecrud\u7684\u4F18\u5316</h2><p data-v-1aa54704>\u7B80\u5355\u6765\u8BF4\uFF0C\u4F60\u76F4\u63A5\u590D\u5236\u4E1A\u52A1\u8868\u683C\u7684\u6587\u4EF6\u5939\uFF0C\u6539\u4E2A\u540D\uFF0C\u6362\u4E2A\u63A5\u53E3\uFF0C\u8C03\u4E00\u4E0B\u91CC\u9762\u7684\u7EC6\u8282\uFF0C\u5C31\u53EF\u4EE5\u76F4\u63A5\u4F7F\u7528\u4E86</p><p data-v-1aa54704>\u4E1A\u52A1\u8868\u683C\uFF0C\u8FD9\u662F\u672C\u5F00\u6E90\u9879\u76EE\u6700\u6838\u5FC3\u7684\u7406\u5FF5\uFF0C\u6211\u57FA\u4E8E\u65E5\u5E38\u5F00\u53D1\u8FC7\u7A0B\u4E2D\u7684\u60C5\u666F\uFF0C\u5C01\u88C5\u4E86\u4E00\u4E2A\u6838\u5FC3\u7684<b data-v-1aa54704>\u5F39\u7A97\u7EC4\u4EF6</b>\u53CA\u4E00\u4E2A<b data-v-1aa54704>table\u7EC4\u4EF6</b>\u3002</p>",6),f=E(()=>u("h3",null,"\u5F39\u7A97\u7EC4\u4EF6",-1)),b=E(()=>u("p",null,"\u9ED8\u8BA4\u652F\u6301\u62D6\u62FD\uFF0C\u5E76\u5C01\u88C5\u4E86\u4E00\u5957\u4E13\u95E8\u4E0E\u8868\u683C\u7EC4\u4EF6\u8054\u52A8\u7684\u6A21\u5F0F\uFF0C\u4F7F\u7528\u6B64\u6A21\u5F0F\u53EF\u4EE5\u5FEB\u901F\u5F00\u53D1",-1)),x=E(()=>u("h3",null,"\u8868\u683C\u7EC4\u4EF6",-1)),y=E(()=>u("p",null,"\u628A\u5927\u90E8\u5206\u8868\u683C\u4E0E\u5206\u9875\u7684\u903B\u8F91\u5904\u7406\u5230\u516C\u7528\u7EC4\u4EF6\u4E4B\u4E2D\uFF0C\u5F00\u7BB1\u5373\u7528",-1));function g(a,k,T,I,N,S){const A=o("basic-template"),t=o("el-link");return n(),p("div",r,[v,D,u("div",h,[e(A)]),u("article",null,[m,u("p",null,[F("\u5728\u65E5\u5E38\u5F00\u53D1\u4E2D\uFF0C\u589E\u5220\u67E5\u6539\u662F\u4E00\u4E2A\u6838\u5FC3\uFF0C\u4E5F\u662F\u4E00\u4E2A\u6700\u91CD\u8981\u7684\u529F\u80FD\uFF0C\u4E3A\u4E86\u9AD8\u6548\u5F00\u53D1\uFF0C\u5EFA\u8BAE\u4E86\u89E3\u8FD9\u4E24\u4E2A\u7EC4\u4EF6\uFF0C\u5177\u4F53\u7684demo\u53EF\u4EE5\u53C2\u7167\u9875\u9762\uFF1A "),e(t,{type:"primary",href:"/#/pages/crudTable"},{default:C(()=>[F("\u4E1A\u52A1\u8868\u683C")]),_:1}),F("\u3001 "),e(t,{type:"primary",href:"/#/pages/categoryTable"},{default:C(()=>[F("\u5206\u7C7B\u8054\u52A8\u8868\u683C")]),_:1}),F("\u3001 "),e(t,{type:"primary",href:"/#/pages/treeTable"},{default:C(()=>[F("\u6811\u5F62\u8054\u52A8\u8868\u683C")]),_:1})]),f,b,x,y])])}var w=l(_,[["render",g],["__scopeId","data-v-1aa54704"]]);export{w as default};