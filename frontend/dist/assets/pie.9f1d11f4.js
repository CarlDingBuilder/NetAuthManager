import{_ as s,d as t,k as r,y as o,z as u,o as n,c as l,a as i}from"./index.17d3c514.js";import{u as m,i as d,a as c,b as p,c as v,k as _,j as f,h,f as y}from"./echarts.95abd183.js";const b={title:{text:"\u5357\u4E01\u683C\u5C14\u73AB\u7470\u56FE",subtext:"\u7EAF\u5C5E\u865A\u6784",left:"center"},tooltip:{trigger:"item",formatter:"{a} <br/>{b} : {c} ({d}%)"},legend:{left:"center",top:"bottom",data:["rose1","rose2","rose3","rose4","rose5","rose6","rose7","rose8"]},toolbox:{show:!0,feature:{mark:{show:!0},dataView:{show:!0,readOnly:!1},restore:{show:!0},saveAsImage:{show:!0}}},series:[{name:"\u534A\u5F84\u6A21\u5F0F",type:"pie",radius:[20,140],center:["25%","50%"],roseType:"radius",itemStyle:{borderRadius:5},label:{show:!1},emphasis:{label:{show:!0}},data:[{value:40,name:"rose1"},{value:33,name:"rose2"},{value:28,name:"rose3"},{value:22,name:"rose4"},{value:20,name:"rose5"},{value:15,name:"rose6"},{value:12,name:"rose7"},{value:10,name:"rose8"}]},{name:"\u9762\u79EF\u6A21\u5F0F",type:"pie",radius:[20,140],center:["75%","50%"],roseType:"area",itemStyle:{borderRadius:5},data:[{value:30,name:"rose1"},{value:28,name:"rose2"},{value:26,name:"rose3"},{value:24,name:"rose4"},{value:22,name:"rose5"},{value:20,name:"rose6"},{value:18,name:"rose7"},{value:16,name:"rose8"}]}]};const $=t({setup(){const a=r(null);let e=null;return o(()=>{m([c,p,v,_,f,h,y]),e=d(a.value),e.setOption(b),u("resize",()=>e.resize())}),{dom:a}}}),w={class:"layout-container"},E={ref:"dom",class:"chart"};function F(a,e,x,A,g,k){return n(),l("div",w,[i("div",E,null,512)])}var z=s($,[["render",F],["__scopeId","data-v-3434a735"]]);export{z as default};
