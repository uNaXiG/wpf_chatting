   M S B A M L   `   `   `  ����A  =client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:clientclient  5   	       client.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:client     5   	   -   5          � Title$  	聊天室��5   	   $	��521��6   $	��817��6"    � 
ResizeMode$ NoResize=�6.    � WindowStartupLocation$ CenterScreen=�6D    � WindowStyle$
 None=�6i    � Icon$ 	/chat.png��6�   ��5	      �� 5
   
    j� Stretch$ UniformToFill=�6    �� ImageSource$ /bg.jpg��6-   .��5      � .+�5   
   �� $��大廳��6   $
��Left=�6   $	��337��6:   $��	21,27,0,0q�6G   $	��Top=�6Z   $	��782��6r   $��16(�6~   ���	#FFFCFFC9.��5      � -   5         �Name$ grid_1��5      $
��Left=�6#   $	��314��6>   $��5,0,0,0q�6K   $	��Top=�6\   $	��767��6t   .+�5      �� -   5         ��Name$ msg_box��5   ,   	 �� 
IsReadOnly$
	 True��6   $
��Left=�6=   $	��293��6X   
 �� TextWrapping$

 Wrap=�6e   $	��Top=�6y   $	��514��6�   $��	10,10,0,0q�6�   $��14(�6�   ���	#FFF1FF00$��2,2,1,2q�6�   ���	#FFF1F0EB��6�   Z� 6  �� 5      -   5         ��Name$ gb��5   '   $��成員��6   $��534,10,10,55q�63   ���	#FFF1FFC9$��1,1,1,1q�6w   ��6`   Z� .��5      � -   5        $
 grid��5      $��	0,0,-22,0q�6)   5      5      �� 5      -   5         ��Name$ user_lb��5      $��Right=�6)   $�� ��6L   $��609,273,10,0q�6W   $	��Top=�6m   $��12(�6�   $��30��6�   ���	#FFF1FFC9 J� RenderTransformOrigin$ 0.5,0.5&�6�   ���	#FFF1F0EB$��2,2,2,2q�6�   5      5      �� 5   
   -   5        $ msg_contant��5      $
��Left=�6'   $	��129��6B   $��
17,369,0,0q�6O   $

 Wrap=�6c   $	��Top=�6w   $	��731��6�   $��14(�6�   
������� J� Opacity$	 0.9Q�6�   ���	#FF6163DE �� SelectionBrush
 �   �6�   �� 5   
   -   5         ��Name$ close��5      $��X��6    $
��Left=�6,   $��	786,0,0,0q�6G   $	��Top=�6Z   ���	#FF3E3E3E$��16(�6�   $��33��6�   $��24��6�   ���	#FF3E3E3E���	#FFC9FFF66�   �� 5   
   -	   5        $	 bar��5      $��NaXiG Chatting��6   $��Center=�66   $	��Top=�6Z   ���	#FF3E3E3E$��24��6�   $��11(�6�   ���	#FFC9FFF6$��0,0,62,0q�6�   r� 5   
   -
   5         r�Name$ sound��5   /   $��-78,456,831,22q�6   6>   �� 5   
   -   5         ��Name$ send_btn��5      $
��Left=�6"   $��24                ��6=   $��764,474,0,0q�5      $	��Top=�6'   $��24��6?   $��	/send.png��6J   $
��Fill=�6]   �� 5   
   -   5        $	 min��5      $	��–��6   $��Center=�6*   $��Center=�6L   $
��Left=�6p   $��	755,0,0,0q�6�   $	��Top=�6�   ���	#FF3E3E3E$��18(�6�   $��33��6�   $��24��6�   ���	#FF3E3E3E���	#FFC9FFF66  �� 5    
   $"��NaXiG © 2022  Version 1.0.0��6   $
��Left=�66   $��650,498,0,0q�6Q   $	��Top=�6f   $
��Bold"�6~   $��11(�6�   5"      5#      