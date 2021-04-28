export const TEMPLATE_CREATE_LEAD: string = 
`
<table class="MsoTableGrid" border="1" cellspacing="0" cellpadding="0" style="border: none; width: 663px; height: 831px;">
 <tbody style="margin-left: 20px;"><tr style="mso-yfti-irow:0;mso-yfti-firstrow:yes;mso-yfti-lastrow:yes;
  height:398.4pt">
  <td width="479" valign="top" style="width:359.6pt;border:solid windowtext 1.0pt;
  mso-border-alt:solid windowtext .5pt;padding:0in 5.4pt 0in 5.4pt;height:398.4pt" class="">
  <p class="MsoNormal" align="center" style="margin-bottom:0in;margin-bottom:.0001pt;
  text-align:center;line-height:normal"><span style="font-family:&quot;Arial&quot;,sans-serif;
  color:#1155CC;mso-no-proof:yes"><o:p>&nbsp;</o:p></span></p>
  &nbsp; &nbsp; &nbsp;&nbsp;<img src="assets/images/logo.png" alt="logo.png" width="auto" height="auto" style="min-width: 0px; min-height: 0px;" class="e-rte-image e-imgleft">
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#222222"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p><br></o:p></span></p><p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></p>
  <table class="MsoTableGrid" border="1" cellspacing="0" cellpadding="0" width="468" style="width: 647px; background: rgb(242, 242, 242); border: none; height: 122px;">
   <tbody><tr style="mso-yfti-irow:0;mso-yfti-firstrow:yes;mso-yfti-lastrow:yes;
    height:45.65pt">
    <td width="468" valign="top" style="width:350.9pt;border:solid windowtext 1.0pt;
    mso-border-alt:solid windowtext .5pt;padding:0in 5.4pt 0in 5.4pt;
    height:45.65pt" class="">
    <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
    <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
    mso-color-alt:windowtext">Khách hàng tiềm năng {{lead_name}}&nbsp;vừa được
    tạo bởi <b>[{{emp_code}} – {{emp_name}}]</b></span><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></p><p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
    mso-color-alt:windowtext"><b><br></b></span></p>
    <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
    </td>
   </tr>
  </tbody></table>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Thời gian tạo: {{dd}} tháng {{MM}} năm {{yyyy}}<o:p></o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
  <table class="MsoTable15Plain1" border="1" cellspacing="0" cellpadding="0" width="469" style="width: 645px; border: none; height: 271px;">
   <tbody><tr style="mso-yfti-irow:-1;mso-yfti-firstrow:yes;mso-yfti-lastfirstrow:
    yes;height:23.1pt">
    <td width="469" colspan="2" valign="top" style="width:351.7pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;mso-border-alt:
    solid #BFBFBF .5pt;mso-border-themecolor:background1;mso-border-themeshade:
    191;padding:0in 5.4pt 0in 5.4pt;height:23.1pt" class="">
    <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center;
    mso-yfti-cnfc:5"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Chi
    tiết khách hàng tiềm năng</span></b><span style="font-size:13.0pt;
    font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></p>
    <p style="margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:5"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></b></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:0;height:10.95pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt" class="">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:68"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;
    color:black;mso-color-alt:windowtext">Họ và tên </span></b><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64"><font color="#000000" face="Arial, sans-serif"><span style="font-size: 17.3333px;">{{lead_name}}</span></font></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:1;height:11.55pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;
    height:11.55pt">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:4"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Email
    <o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;height:11.55pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt"><font face="Arial, sans-serif"><span style="font-size: 17.3333px;">{{lead_email}}</span></font></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:2;height:10.95pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt" class="">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:68"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;
    color:black;mso-color-alt:windowtext">Số điện thoại </span></b><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
    mso-color-alt:windowtext">{{lead_phone}}</span><span style="font-size:13.0pt;
    font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:3;height:11.55pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;
    height:11.55pt" class="">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:4"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Địa
    chỉ <o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;height:11.55pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt"><font face="Arial, sans-serif"><span style="font-size: 17.3333px;">{{lead_address}}</span></font></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:4;height:11.55pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:11.55pt">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:68"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;
    color:black;mso-color-alt:windowtext">Nhu cầu sản phẩm dịch vụ</span></b><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:11.55pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64"><font color="#000000" face="Arial, sans-serif"><span style="font-size: 17.3333px;">{{lead_interested}}</span></font></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:5;height:10.95pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:4"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Mức
    độ tiềm năng <o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;padding:0in 5.4pt 0in 5.4pt;height:10.95pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt"><font face="Arial, sans-serif"><span style="font-size: 17.3333px;">{{lead_potential}}</span></font></p>
    </td>
   </tr>
   <tr style="mso-yfti-irow:6;mso-yfti-lastrow:yes;height:10.95pt">
    <td width="189" valign="top" style="width:141.9pt;border:solid #BFBFBF 1.0pt;
    mso-border-themecolor:background1;mso-border-themeshade:191;border-top:
    none;mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:
    background1;mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt">
    <p align="right" style="margin:0in;margin-bottom:.0001pt;text-align:right;
    mso-yfti-cnfc:68"><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;
    color:black;mso-color-alt:windowtext">Người phụ trách </span></b><b><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></b></p>
    </td>
    <td width="280" valign="top" style="width:209.8pt;border-top:none;border-left:
    none;border-bottom:solid #BFBFBF 1.0pt;mso-border-bottom-themecolor:background1;
    mso-border-bottom-themeshade:191;border-right:solid #BFBFBF 1.0pt;
    mso-border-right-themecolor:background1;mso-border-right-themeshade:191;
    mso-border-top-alt:solid #BFBFBF .5pt;mso-border-top-themecolor:background1;
    mso-border-top-themeshade:191;mso-border-left-alt:solid #BFBFBF .5pt;
    mso-border-left-themecolor:background1;mso-border-left-themeshade:191;
    mso-border-alt:solid #BFBFBF .5pt;mso-border-themecolor:background1;
    mso-border-themeshade:191;background:#F2F2F2;mso-background-themecolor:
    background1;mso-background-themeshade:242;padding:0in 5.4pt 0in 5.4pt;
    height:10.95pt" class="">
    <p style="margin:0in;margin-bottom:.0001pt;mso-yfti-cnfc:64"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:black;
    mso-color-alt:windowtext">[{{pic_code}} – {{pic_name}}]</span><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p></o:p></span></p>
    </td>
   </tr>
  </tbody></table>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif">Nhấn vào link bên
  dưới để xem chi tiết thông tin [{{lead_name}}] <o:p></o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:13.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><img src="assets/images/access-button.png" class="e-rte-image e-imginline e-resize" alt="access-button.png" width="auto" height="auto" style="min-width: 0px; min-height: 0px;"><br></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></p>
  <p align="center" style="margin:0in;margin-bottom:.0001pt;text-align:center"><span style="font-size:9.0pt;font-family:&quot;Arial&quot;,sans-serif;color:#7F7F7F;
  mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></p>
  <p class="MsoNormal" align="center" style="margin-top:0in;margin-right:3.75pt;
  margin-bottom:0in;margin-left:0in;margin-bottom:.0001pt;text-align:center;
  line-height:normal;background:#F4F4F4"><b><span style="font-family:&quot;Arial&quot;,sans-serif;
  color:#7F7F7F;mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></b></p>
  <p class="MsoNormal" align="center" style="margin-top:0in;margin-right:3.75pt;
  margin-bottom:0in;margin-left:0in;margin-bottom:.0001pt;text-align:center;
  line-height:normal;background:#F4F4F4"><b><span style="font-family:&quot;Arial&quot;,sans-serif;
  color:#7F7F7F;mso-themecolor:text1;mso-themetint:128">{{company_name}}]<br>
  [{{company_address}}]<o:p></o:p></span></b></p>
  <p class="MsoNormal" align="center" style="margin-top:0in;margin-right:3.75pt;
  margin-bottom:0in;margin-left:0in;margin-bottom:.0001pt;text-align:center;
  line-height:normal;background:#F4F4F4"><b><span style="font-family:&quot;Arial&quot;,sans-serif;
  color:#7F7F7F;mso-themecolor:text1;mso-themetint:128"><o:p>&nbsp;</o:p></span></b></p>
  </td>
 </tr>
</tbody></table>

<div><!--[if !supportAnnotations]-->

<div><div id="_com_1" class="msocomtxt" language="JavaScript"><p class="MsoNormal" align="center" style="text-align:center"><br></p><p class="MsoListParagraph" style="margin-left:0in;mso-add-space:auto"><o:p></o:p></p>

<p class="MsoCommentText"><o:p>&nbsp;</o:p></p>

<p class="MsoCommentText"><o:p>&nbsp;</o:p></p>

<!--[if !supportAnnotations]--></div>

<!--[endif]--></div>

</div><!---->
`
;