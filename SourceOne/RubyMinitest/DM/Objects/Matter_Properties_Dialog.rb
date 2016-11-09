require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class MatterPropertiesDialog < BrowserContainer
   
  #set mattername.
  def set_MatterName(mattetrname)
    matter_name_tb.wait_until_present
    matter_name_tb.set(mattetrname)
  end
  
  #set matter description.
  def set_Matterdes(mattetrname)
    matter_des_tb.wait_until_present
    matter_des_tb.set(mattetrname)
  end
  
  #click matter hold folder button.
  def set_Matter_holdfolder
    matter_holdfolder_bt.wait_until_present
    matter_holdfolder_bt.click
  end
  
  #select the hold folder.
  def select_holdfolder(holdfolder)
    selectholdfolder = BtnIDHelper.new(@browser)
    selectholdfolder.get_Dlg_btnID("Hold Folders",holdfolder)
    selectholdfolder.get_Dlg_btnID("Hold Folders","OK")
    sleep(3)
  end
  
  
  #remove the staff
  def remove_staff(dialogname,user)
    removestaff = BtnIDHelper.new(@browser)
    fulluser=user+' ('+user+'@'+$_UserDomain+')'
    removestaff.get_Dlg_btnID(dialogname,fulluser.capitalize)
    removestaff.get_Dlg_btnID(dialogname,"Remove")
    removestaff.get_Dlg_btnID("Confirmation","Yes")
    sleep(3)
    
  end
  
  #add the staff
  def add_staff(dialogname,user)
    addstaff = BtnIDHelper.new(@browser)
    addstaff.get_Dlg_btnID(dialogname,"Add")
    fulluser=user+' ('+user+'@'+$_UserDomain+')'
    addstaff.get_Dlg_btnID("Users",fulluser.capitalize)
    addstaff.get_Dlg_btnID("Users","OK")
    sleep(3)
    
  end
  
  #verify the user exist
  def verify_user(dialogname)
    @browser.div(:id=>dialogname).span(:text=>$deleteUserName1).exist?
  end
  
  #keywords 
  def input_keywords(dialogname,propertiesname)
     root=(@browser.span(:text =>dialogname,:class=>"dijitDialogTitle").id).gsub('_title','')
     @browser.div(:id=>root).span(:class =>"dijitInline dijitTabStripIcon dijitTabStripSlideRightIcon").click
     newtag_bt= BtnIDHelper.new(@browser)
     newtag_bt.get_Dlg_btnID(dialogname,"Keywords")
     @browser.text_field(:id => "dijit_form_Textarea_0").set(propertiesname)
  end
  
  #delete custodian
  def delete_custodian(dialogname,custodianname)
    newtag_bt= BtnIDHelper.new(@browser)
    newtag_bt.get_Dlg_btnID(dialogname,custodianname)
    @browser.span(:class=>"dijitReset dijitInline dijitIcon MatterCustodian_Button_Delete").parent.span(:text=>"Delete").click
    newtag_bt.get_Dlg_btnID("Confirmation","Yes")
  end
  
  
  
  #edit custodian
  def edit_custodian(dialogname,custodianname,custodiandes)
    newtag_bt= BtnIDHelper.new(@browser)
    newtag_bt.get_Dlg_btnID(dialogname,custodianname)
    @browser.span(:class=>"dijitReset dijitInline dijitIcon MatterCustodian_Button_Edit").parent.span(:text=>"Edit").click
    @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)
    newtag_bt.get_Dlg_btnID("Custodian","OK")
  end
  
  
  
  #new custodian
  def new_custodian(dialogname,type,userstring,user,custodiandes)
    @browser.span(:class=>"dijitReset dijitInline dijitIcon MatterCustodian_Button_New").parent.span(:text=>"New").click
    tagroot=(@browser.span(:text => "Custodian",:class=>"dijitDialogTitle").id).gsub('_title','')
    @browser.div(:id=>tagroot).div(:class =>"dijitReset dijitArrowButtonInner").click
    @browser.table(:id=>"dijit_Menu_1").rows.each{
    |x|if x.attribute_value("aria-label").rstrip==type.rstrip 
          x.click
          #x.focus
          #@browser.send_keys :enter
      end }
    if type=="Exchange"
      @browser.text_field(:id => "dijit_form_ValidationTextBox_1").set(userstring) 
      @browser.div(:id=>tagroot).span(:text =>"Find").click
      if @browser.span(:text=>"Error").exist?
      puts "address list have not the usestring"
      elsif @browser.span(:text=>"Address List").exist?
      addressList= BtnIDHelper.new(@browser)
      addressList.get_Dlg_btnID("Address List",user)
      addressList.get_Dlg_btnID("Address List","OK")
      @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)
      else
      @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)
     end
    elsif type=="Identity"
      @browser.text_field(:id => "dijit_form_ValidationTextBox_1").set(userstring) 
      @browser.div(:id=>tagroot).span(:text =>"Find").click
      if @browser.span(:text=>"Error").exist?
      puts "address list have not the usestring"
      elsif @browser.span(:text=>"Address List").exist?
      addressList= BtnIDHelper.new(@browser)
      addressList.get_Dlg_btnID("Address List",user)
      addressList.get_Dlg_btnID("Address List","OK")
      @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)
      else
      @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)
      end
    else
      @browser.text_field(:id => "dijit_form_ValidationTextBox_1").set(user)
      @browser.text_field(:id => "dijit_form_Textarea_1").set(custodiandes)  
    end
    newtag_dlg= BtnIDHelper.new(@browser)
    newtag_dlg.get_Dlg_btnID("Custodian","OK")
  end
  
  
  #delete tag
  def delete_tag(dialogname,tagname)
    newtag_bt= BtnIDHelper.new(@browser)
    newtag_bt.get_Dlg_btnID(dialogname,tagname)
    newtag_bt.get_Dlg_btnID(dialogname,"Delete")
    deletetag_dlg= BtnIDHelper.new(@browser)
    deletetag_dlg.get_Dlg_btnID("Confirmation","Yes")
  end
  
  #find the tag by value
  def findtag(value)
    @browser.span(:title=>value)
  end
  
  #edit tag
  def edit_tag(dialogname,tagname,newtagname,tagdes,tagcolor)
    newtag_bt= BtnIDHelper.new(@browser)
    newtag_bt.get_Dlg_btnID(dialogname,tagname)
    newtag_bt.get_Dlg_btnID(dialogname,"Edit")
    newtag_dlg= BtnIDHelper.new(@browser)
    @browser.text_field(:id => "dijit_form_ValidationTextBox_1").set(newtagname)
    @browser.text_field(:id => "dijit_form_Textarea_1").set(tagdes)
    tagroot=(@browser.span(:text => "Edit Tag",:class=>"dijitDialogTitle").id).gsub('_title','')
    @browser.div(:id=>tagroot).input(:class =>"dijitReset dijitInputField dijitArrowButtonInner").click
    @browser.table(:id=>"DiscoveryManager/Common/Widgets/Select_0_menu").rows.each{
    |x|if x.attribute_value("aria-label")==tagcolor 
          x.click
          #x.focus
          #@browser.send_keys :enter
      end }
    newtag_dlg.get_Dlg_btnID("Edit Tag","OK")
  end
  
  
  #new tag
  def new_tag(dialogname,tagname,tagdes,tagcolor)
    newtag_bt= BtnIDHelper.new(@browser)
    newtag_bt.get_Dlg_btnID(dialogname,"New")
    newtag_dlg= BtnIDHelper.new(@browser)
    @browser.text_field(:id => "dijit_form_ValidationTextBox_1").set(tagname)
    sleep(1)
    @browser.text_field(:id => "dijit_form_Textarea_1").set(tagdes)
    sleep(1)
    tagroot=(@browser.span(:text => "New Tag",:class=>"dijitDialogTitle").id).gsub('_title','')
    @browser.div(:id=>tagroot).input(:class =>"dijitReset dijitInputField dijitArrowButtonInner").click
    @browser.table(:id=>"DiscoveryManager/Common/Widgets/Select_0_menu").rows.each{
    |x|if x.attribute_value("aria-label")==tagcolor 
          x.click
          #x.focus
          #@browser.send_keys :enter
      end }
    newtag_dlg.get_Dlg_btnID("New Tag","OK")
  end
  
  
  #select search folder
  def click_searchfolder_bt(dialogname,foldercollection)
    @browser.send_keys :tab
    @browser.send_keys :space
    sleep(3)
    foldersselect= BtnIDHelper.new(@browser)
    foldercollection.each {|folder|foldersselect.get_Dlg_btnID(dialogname,folder)}
 end
  
  #closed or open the matter
 def matter_state(dlg_name,state)
   matterproroot=(@browser.span(:text => dlg_name,:class=>"dijitDialogTitle").id).gsub('_title','')
   @browser.div(:id=>matterproroot).input(:class =>"dijitReset dijitInputField dijitArrowButtonInner").click
   if state=="closed"
     sleep(3)
     @browser.send_keys :down
     sleep(3)
     @browser.send_keys :enter
   else
    sleep(3)
    @browser.send_keys :up
    sleep(3)
    @browser.send_keys :enter
   end
 end

 #New matter Hold Folder browser button.
  def matter_holdfolder_bt
     @browser.span(:id => "dijit_form_Button_4_label")
  end 
  
   #New matter description text box.
  def matter_des_tb
     @browser.text_field(:id => "input_newmatter_description")
  end 
  
  #New matter name text box.
  def matter_name_tb
     @browser.text_field(:id => "input_newmatter_name")
  end 
  

end