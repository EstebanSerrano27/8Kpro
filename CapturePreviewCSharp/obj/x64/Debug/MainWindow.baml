   M S B A M L   `   `   `  ����W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Y>clr-namespace:System.Windows.Interop;assembly=PresentationCoreSystem.Windows.Interop  5   	   O KCapturePreviewCSharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:CapturePreviewCSharpCapturePreviewCSharp 5   	   %   CapturePreviewCSharp.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml      5   	   @d2http://schemas.microsoft.com/expression/blend/2008      5   	   Li>clr-namespace:System.Windows.Interop;assembly=PresentationCore      5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006      5   	   4local"clr-namespace:CapturePreviewCSharp      5   	   -   5          � Title$  Capture Preview��5	   	   $	��475��6!   $
��1156��6.   .��5
      ]� ��. .+�5   
   � $
��Left=�6   $	��360��6%    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� -   5         ��Name$ groupBoxProperties��5      $��
Properties��63   .��5      � $
��Left=�6   $��5q�62   $	��340��6=    5      �� 5      $
��Auto �6(   �� 5      $
��Auto �6(   �� 5      $
��Auto �6(   �� 5      $
��Auto �6(   �� 5      $��* �6(    � ColumnDefinitions 5      �� 5      $
��Auto �6+   �� 5      $��* �6+   .+�5      �� -   5         ��Name$ labelDevice��5      $��Input Device:��61   $��3q�6I   �� 5       -   5         ��Name$ comboBoxDevice��5       $��1��67   $��3q�6G   6R   �� 5!      -   5        $ labelConnection��5!      $��Input Connection:��65   $��1��6Q   $��3q�6^   �� 5"      -   5        $ comboBoxConnection��5"      $��1��6;   $��1��6I   $��3q�6Y   6d   �� 5#      -   5        $ labelFormat��5#      $��Video Format:��61   $��2��6I   $��3q�6V   �� 5$      -   5        $ comboBoxVideoFormat��5$      $��2��6<   $��1��6J   $��3q�6Z   6e   �� 5%      -	   5        $ labelAutoDetect��5%      $��Auto-Detect Format:��65   $��3��6S   $��3q�6`   $y�	Collapsed=�6k   �� 5&      -
   5         ��Name$ checkBoxAutoDetect��5&      $��3��6;   $��1��6H   $
��Left=�6X   $��Center=�6s   $��3q�6�   $y�	Collapsed=�6�    d� IsThreeState .  6
  �� 5'      -   5        $ label3DPreview��5'      $��3D Preview Format:��64   $��4��6Q   $��3q�6^   $y�	Collapsed=�6i   �� 5(      -   5        $ comboBox3DPreviewFormat��5(      $��4��6@   $��1��6N   $��3q�6^   $y�	Collapsed=�6i   5)      5*      �� 5+      -   5        $ groupBoxTimecode��5+      $��Timecode��61   $��1��6C   .��5,      &   System.Windows.Controls.DataGrid  -   5        $��dataGridTimecode��5,         AutoGenerateColumns .  	   GridLinesVisibility$
	 None=�6t   "
   VerticalScrollBarVisibility$

 Auto=�6�   $   HorizontalScrollBarVisibility$ Disabled=�6�      HeadersVisibility$ Column=�6�   
������    RowBackground
 ����    CanUserAddRows .  ��63   �� *TimecodeData+   Columns 5-      0  *System.Windows.Controls.DataGridTextColumn  5.      ,  &System.Windows.Controls.DataGridColumn   Header$ Format��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$	 125 6X   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6=   �� *Format+6X     5/      $ Value��6-   $	 125 6V    6<   �� *Value+6V     50      $ 	User Bits��6-   $ * 6]    6@   �� *
UserBits+6]   52      53      �� 54      -   5        $ groupBoxVANC��54      $
��VANC��6-   $��2��6;   .��55        -   5        $��dataGridVANC��55       .  $
	 None=�6r   $

 Auto=�6�   $
 Auto=�6�   $ Column=�6�   
������ 
 ����  .  ��6/   �� *VANCPacketData+ 56        57      $
 Line��6-   $ 40 6T    6;   �� *Line+6T     58      $	 DID��6-   $ 40 6R    6:   �� *DID+6R     59      $
 SDID��6-   $ 40 6T    6;   �� *SDID+6T     5:      $
 Data��6-   $
 Auto 6T      MinWidth$	 226Q�6a    6;   �� *Data+6a   5<      5=      �� 5>      -   5        $ groupBoxMetadata��5>      $��Static Metadata��61   $��3��6J   .��5?        -   5        $��dataGridMetadata��5?       .  $
	 None=�6t   $

 Auto=�6�   $ Disabled=�6�   $ Column=�6�   
������ 
 ����  .  ��63   �� *MetadataData+ 5@        5A      $
 Item��6-   $	 250 6T    6;   �� *Item+6T     5B      $ Value��6-   $ * 6V    6<   �� *Value+6V   5D      5E      5F      �� 5G   
   -   5        $ groupBoxPreview��5G      $��Preview��6,   .��5H      � -   5         �Name$ gridPreview��5H      $��5q�6(   .+�5I      �� -   5         ��Name$ imagePreview��5I      ��5J      %   System.Windows.Interop.D3DImage  5K      -   5        
d3dPreview��5K   %   5M      5N      5O      5P      5Q      