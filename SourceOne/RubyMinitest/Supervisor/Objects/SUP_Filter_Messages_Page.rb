require 'watir-webdriver'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
require_relative '..\Common\Date_Verify.rb'
gem "minitest"
require "minitest/autorun"

#Filter dialog box page==============================================================
class SUPFilterMessagePage < BrowserContainer

  #verify the filter dialog box pop up.
  def verifyFilterDialog
    if filter_dialogbox.exists?
      return true
    else
      puts "Filter dialog box cannot be found."
      return false
    end
  end


  #Set the "Show messages that meet the following conditions".
  def setShowCondition(s_conditions)
    filter_condition = s_conditions 
    
    if filter_condition[0] != "no"
      #check Review incomplete, messages dated and select a value.
      checkReviewIncomplete(true)
      selectDated(review_incomplete_sel_list, filter_condition[0])
    else
      checkReviewIncomplete(false)  
    end
    
    if filter_condition[1] != "no"
      #check Review complete, messages dated and select a value.
      checkReviewComplete(true)
      selectDated(review_complete_sel_list, filter_condition[1])
    else
      checkReviewComplete(false)  
    end    
  
    if filter_condition[2] != "no"
      #check Escalated messages.
      checkReviewEscalted(true)
    else
      checkReviewEscalted(false)  
    end         
    
    if filter_condition[3] != "no"
      #check Escalated messages.
      checkReviewDateRange(true)
      #set the start date.
      setDate(dated_between_start_textbox, filter_condition[3])
      if DateValidation.date_verify(filter_condition[3]) == false
        puts"DateValidation failed"
        return false
      end
      #set the end date.
      setDate(dated_between_end_textbox, filter_condition[4])
      
    else
      checkReviewDateRange(false)  
    end  
    
    #check the specified review groups.
    clearRG
    checkRG(filter_condition[5])     
    
    #save and close the dialog.
    close_filter_dialog
    
    #verify start date must less than end date.
    if DateValidation.compare_date(filter_condition[3], filter_condition[4]) ==false
      puts"DateValidation compare failed"
      return false
    end
    @browser.wait($TIMEOUT)
    
  end

  #verify whether filter changes are saved.
  def verifyFilterChange(excel_data)
    #verify
    assert_switch = "true"
    #verify Review incomplete, messages dated.
    if excel_data[0] == "no"
      if review_incomplete_chk.attribute_value("aria-checked").to_s == "false"
        puts "The Review incomplete, messages dated is uncheked, it is as expected."
        if review_incomplete_sel_list.class_name.to_s.include?"dijitDisabled"
          puts "The Review incomplete, messages dated select list is disabled."
        else
          puts "error: The Review incomplete, messages dated select list is enabled. But the check box is unchecked."
        end
      else
        puts "error: the Review incomplete, messages dated is cheked. But the expected reuslt is unchecked."
        assert_switch += "false"
      end
    else
      if review_incomplete_chk.attribute_value("aria-checked").to_s == "true"
        puts "The Review incomplete, messages dated is cheked, it is as expected."
        if review_incomplete_sel_list.attribute_value("textContent").to_s == excel_data[0]
          puts "The expected Review incomplete, messages dated is "+excel_data[0]
        else
          puts "error: The expected Review incomplete, messages dated is "+excel_data[0]+". But the actual result is "+review_incomplete_sel_list.attribute_value("textContent").to_s
          assert_switch += "false"
        end
      else
        puts "error: the Review incomplete, messages dated is uncheked. But the expected reuslt is checked."
        assert_switch += "false"
      end
    end
    
    #verify Review complete, messages dated.
    if excel_data[1] == "no"
      if review_complete_chk.attribute_value("aria-checked").to_s == "false"
        puts "the Review complete, messages dated is uncheked, it is as expected."
        if review_complete_sel_list.class_name.to_s.include?"dijitDisabled"
          puts "The Review complete, messages dated select list is disabled."
        else
          puts "error: The Review complete, messages dated select list is enabled. But the check box is unchecked."
        end        
      else
        puts "error: the Review complete, messages dated is cheked. But the expected reuslt is unchecked."
        assert_switch += "false"
      end
    else
      if review_complete_chk.attribute_value("aria-checked").to_s == "true"
        puts "The Review complete, messages dated is cheked, it is as expected."
        if review_complete_sel_list.attribute_value("textContent").to_s == excel_data[1]
          puts "The expected Review complete, messages dated is "+excel_data[1]
        else
          puts "error: The expected Review complete, messages dated is "+excel_data[1]+". But the actual result is "+review_complete_sel_list.attribute_value("textContent").to_s
          assert_switch += "false"
        end
      else
        puts "error: the Review complete, messages dated is uncheked. But the expected reuslt is checked."
        assert_switch += "false"
      end
    end    

    #verify the Escalated messages.
    if excel_data[2] == "no"
      if review_escalted_chk.attribute_value("aria-checked").to_s == "false"
        puts "The Escalation messages is uncheked, it is as expected."
      else
        puts "error: the Escalation messages is cheked. But the expected reuslt is unchecked."
        assert_switch += "false"
      end
    else
      if review_escalted_chk.attribute_value("aria-checked").to_s == "true"
        puts "The Escalation messages is cheked, it is as expected."
      else
        puts "error: the Escalation messages is uncheked. But the expected reuslt is checked."
        assert_switch += "false"
      end      
    end

    #verify the Messages dated between.
    if excel_data[3] == "no"
      if review_date_range_chk.attribute_value("aria-checked").to_s == "false"
        puts "The Messages dated between is uncheked, it is as expected."
      else
        puts "error: the Messages dated between is cheked. But the expected reuslt is checked."
        assert_switch += "false"
      end
    else
      if review_date_range_chk.attribute_value("aria-checked").to_s == "true"
        puts "The Messages dated between is cheked, it is as expected."
        if dated_between_start_textbox.attribute_value("value").to_s == excel_data[3]
          puts "Start date is as expected."
        else
          puts "error: the expected start date is "+excel_data[3]+". But the actual is "+dated_between_start_textbox.attribute_value("value").to_s
          assert_switch += "false" 
        end
        if dated_between_end_textbox.attribute_value("value").to_s == excel_data[4]
          puts "End date is as expected."
        else
          puts "error: the expected end date is "+excel_data[4]+". But the actual is "+dated_between_end_textbox.attribute_value("value").to_s
          assert_switch += "false" 
        end        
      else
        puts "error: the Messages dated between is uncheked. But the expected reuslt is checked."
        assert_switch += "false"        
      end
    end
    
    #verify the review group.
    rg = checkedRG
    if excel_data[5] =="all"
      #get the checked review group number.
      rg_num = rg.size
      #get the total review group number.
      rg_total = getRGTotalNumber
      if rg_num == rg_total
        puts "The review groups are as expected when check all."
      else
        puts "error: it checked all review group. But not all the review groups are checked."
        assert_switch += "false"
      end
    else
      if excel_data[5].split(/,/).sort == rg.sort
        puts "The review groups are as expected."
      else
        puts "error: the expected review group is "+excel_data[5]+". But the actual result is "+rg.join(",")
        assert_switch += "false"
      end        
    end
    
    #close filter dialog box.
    clickCancel
    
    if assert_switch.include?"false"
      return false
    else
      return true
    end
    
  end

  #clear all the checked review groups in the RG list.
  def clearRG
    if review_group_all_chk.exists?
      if review_group_all_chk.attribute_value("innerHTML").to_s.include?"cleared"
        review_group_all_chk.click
        review_group_all_chk.click
      else
        review_group_all_chk.click
      end
    else
      puts "error: The checkbox for all the review groups cannot be found."
    end
  end

  #click on OK button to close the dialog box.
  def close_filter_dialog
    if ok_btn.exists?
      ok_btn.click
    else
      puts "error: the OK button cannot be found on the filter dialog box."
      return false
    end  
  end 

  #get the count number for all the review groups.
  def getRGTotalNumber
    scrollEnd
    if review_group_chk_list.exists?
      sleep(3)
      review_group_chk_list.attribute_value("childElementCount").to_i
    else
      puts "error: The checkboxes for the review groups cannot be found."
      return false
    end
  end

  #scroll to the end.
  def scrollEnd
    if virtual_scroll.exists?
      if virtual_scroll.visible?
        virtual_scroll.focus
        puts virtual_scroll.click
        height = virtual_scroll.attribute_value("scrollTopMax").to_i
        top = virtual_scroll.attribute_value("scrollTop").to_i
        while top < height
          @browser.send_keys :down
          top = virtual_scroll.attribute_value("scrollTop").to_i
          sleep(1)
        end
      end
    else
      puts "error: Virtual scroll cannot be found in the Filter dialog box."
      return false
    end  
  end
  
  #scroll to the top.
  def scrollTop
    if virtual_scroll.exists?
      if virtual_scroll.visible?
        virtual_scroll.focus
        top = virtual_scroll.attribute_value("scrollTop").to_i
        while top != 0
          @browser.send_keys :up
          top = virtual_scroll.attribute_value("scrollTop").to_i
          sleep(1) 
        end   
      end 
    else 
      puts "error: Virtual scroll cannot be found in the Filter dialog box."    
    end
  end

  #get the checked review groups.
  def checkedRG
    #get the total review groups in the filter dialog box.
    rg_num = getRGTotalNumber-1
    #go to the top.
    scrollTop
    checked = []
    #check the specified review group.
    for i in 0..rg_num
      if i+1 >4 && i%4==0
        virtual_scroll.focus
        @browser.send_keys :down
        @browser.send_keys :down
      end  
      if /gridxRowSelected/ =~ single_RG_row(i).class_name.to_s
        checked << single_RG_row(i).attribute_value("textContent").to_s
      end
    end 
    return checked
  end
  
  #check the specified review groups.
  def checkRG(rg_name)
    name = rg_name
    if name == "all"
      #click check all.
      review_group_all_chk.click
      return
    end
    #convert string to array.
    array_name = name.split(/,/)
    name_size = array_name.size-1
    #get the total review groups in the filter dialog box.
    rg_num = getRGTotalNumber-1
    #go to the top.
    scrollTop
    #check the specified review group.
    for i in 0..rg_num
       if i+1 >4 && i%4==0
         virtual_scroll.focus
         @browser.send_keys :down
         @browser.send_keys :down
       end
       for j in 0..name_size
        if single_RG_row(i).attribute_value("textContent").to_s == array_name[j]
          single_RG_row(i).click
        end         
       end      
    end 
  end  

  #check or uncheck the checkbox of Review incomplete, messages dated.
  def checkReviewIncomplete(check_or_uncheck)
    checkMessagesCondition(review_incomplete_chk, check_or_uncheck)
  end
  
  #check or uncheck the checkbox of Review complete, messages dated.
  def checkReviewComplete(check_or_uncheck)
    checkMessagesCondition(review_complete_chk, check_or_uncheck)
  end
    
  #check or uncheck the checkbox of Escalated messages.
  def checkReviewEscalted(check_or_uncheck)
    checkMessagesCondition(review_escalted_chk, check_or_uncheck)
  end
  
  #check or uncheck the checkbox of Escalated messages.
  def checkReviewDateRange(check_or_uncheck)
    checkMessagesCondition(review_date_range_chk, check_or_uncheck)
  end

  #check the checkboxes in filter dialog box for Show messages that meet the following conditions.
  #Parameter: _boolean: true means check; false means uncheck
  def checkMessagesCondition(object, _boolean)
    if object.exists?
      if object.attribute_value("aria-checked").to_s != _boolean.to_s
        object.click
      end
    else
      puts "error: It cannot find the checkbox of "+object.id.to_s+"."
      return false  
    end    
  end

  #select expected dated.
  def selectDated(object, inputArray)
    if object.exists?
      if object.class_name.to_s.include?"dijitSelectDisabled"
        puts "error: "+object.id.to_s+" select list is disabled."
      else
        current_selected = object.attribute_value("textContent").to_s
        while current_selected != inputArray
          object.focus
          @browser.send_keys :enter
          @browser.send_keys :down
          @browser.send_keys :enter
          current_selected = object.attribute_value("textContent").to_s
        end    
      end
      
    end
  end

  #set date.
  def setDate(date_object, inputDate)
    if date_object.exists?
      if date_object.attribute_value("aria-disabled").to_s == "false"
        if date_object.attribute_value("value").to_s != inputDate
          #clear the date.
          date_object.click
          @browser.send_keys [:control, 'a'], :backspace 
          #set the date.
          date_object.send_keys(inputDate)
        end
      else
        puts "error: "+date_object.id.to_s+" is disabled."    
      end
    else
      puts "error: "+date_object.id.to_s+" cannot be found."  
    end
  end

  #verify the validation warning tips for the start date can be found.
  def verifyWarningStartDate
    if validation_startdate.exists?
      return true
    else
      return false
    end
  end
  
  def verifyWarningEndDate
    if validation_enddate.exists?
      return true
    else
      return false
    end
  end 

  #click on Cancel button.
  def clickCancel
    if cancel_button.exists?
      cancel_button.click
    end
  end

  #get warning info
  def getWarningInfo
    if warning_dialog.exists?
      return warning_dialog.attribute_value("textContent").to_s
    else
      return false
    end
  end

  private

  #The check box of input_show_at_startup
  def input_show_at_startup_chk
    @browser.div(:id => "input_show_at_startup")
  end

  #The OK button.
  def ok_btn
    @browser.span(:class => "dijitReset dijitStretch dijitButtonContents")
  end

  #The checkboxes for the review groups.
  def review_group_chk_list
    @browser.div(:class => "dijitDialog").div(:class => "gridxRowHeaderBody")
  end
  
  #The 'check all' checkbox for the review group.
  def review_group_all_chk
    @browser.div(:class => "dijitDialog").div(:class => "gridxRowHeaderHeader")
  end
  
  #virtual scroll 
  def virtual_scroll
    @browser.div(:class => "dijitDialog").div(:class => "gridxVScroller")
  end
  
  #specified check box of review group.
  def single_RG_chk(i_record)
    @browser.div(:class => "dijitDialog").div(:class => "gridxRowHeaderRow", :index => i_record)
  end
  
  #review group list
  def review_group_list
    @browser.div(:class => "dijitDialog").div(:class => "gridxBody gridxBodyRowHoverEffect")
  end
  
  #specified row of review group.
  def single_RG_row(i_row)
    @browser.div(:class => "dijitDialog").div(:class => "gridxRow", :index => i_row)
  end  

  #the checkbox of Review incomplete, messages dated.
  def review_incomplete_chk
    @browser.input(:id => "input_review_incomplete")
  end
  
  #the checkbox of Review complete, messages dated.
  def review_complete_chk
    @browser.input(:id => "input_review_complete")
  end
    
  #the checkbox of Escalated messages.
  def review_escalted_chk
    @browser.input(:id => "input_review_escalted")
  end
      
  #the checkbox of Messages dated between.
  def review_date_range_chk
    @browser.input(:id => "input_review_date_range")
  end

  #the select list of Review incomplete, messages dated.
  def review_incomplete_sel_list
    @browser.table(:id => "input_review_incomplete_range")
  end

  #the select list of Review complete, messages dated.
  def review_complete_sel_list
    @browser.table(:id => "input_review_complete_range")
  end

  #the messages dated between--Start.
  def dated_between_start_textbox
     if @browser.input(:id => "dijit_form_DateTextBox_0").exists?
       @browser.input(:id => "dijit_form_DateTextBox_0")
     else
       @browser.input(:id => "dijit_form_DateTextBox_2")
     end
  end

  #the messages dated between--End.
  def dated_between_end_textbox
    if @browser.input(:id => "dijit_form_DateTextBox_1").exists?
      @browser.input(:id => "dijit_form_DateTextBox_1")
    else
      @browser.input(:id => "dijit_form_DateTextBox_3")
    end
  end

  #the filter dialog box
  def filter_dialogbox
    @browser.div(:class => "dijitDialog")
  end    
  
  #The validation warning tips for the start date.
  def validation_startdate
    @browser.div(:class =>"filterItem", :index=>3).div(:class=>/dijitValidationTextBox/, :index=>0).div(:class=>"dijitReset dijitValidationContainer")
  end
  
  #The validation warning tips for the end date.
  def validation_enddate
    @browser.div(:class =>"filterItem", :index=>3).div(:class=>/dijitValidationTextBox/, :index=>1).div(:class=>"dijitReset dijitValidationContainer")
  end

  #Cancel button.
  def cancel_button
    @browser.span(:class =>"dijitDialogCloseIcon")
  end    
  
  #error pop up dialog box.
  def warning_dialog
    @browser.p(:id =>/errorDialogMessage/)
  end
  
end 


