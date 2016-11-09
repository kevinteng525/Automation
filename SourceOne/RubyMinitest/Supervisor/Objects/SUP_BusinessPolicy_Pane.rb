require 'watir-webdriver'
require_relative '..\Variables\SUP_Variables_Configure.rb'
require_relative '..\Objects\Supervisor_Objects.rb'
require_relative '..\Objects\SUP_MessageList_Page.rb'
gem "minitest"
require "minitest/autorun"

class SUPBusinessPolicyPane < SUPMessageListPage
  
  def getBPRow
    if bp_table.exists?
      return bp_table.attribute_value("childElementCount").to_i
    else
      puts "Function 'getBPRow' of SUP_BusinessPolicy_Pane.rb - error: BP table cannot be found."
    end
  end  
  
  #get the assign code status.
  def getBPCodeName(irow)
    if bp_row(irow.to_i-1).exists?
      return bp_row(irow.to_i-1).attribute_value("title")
    else
      puts "Function 'getBPCodeName' of SUP_BusinessPolicy_Pane.rb - error: BP code name cannot be found."
    end
  end
  
  #click assign code icon.
  def clickBPIcon(irow)
    if bp_mark_message(irow.to_i-1).exists?
      bp_mark_message(irow.to_i-1).click
    else
      puts "Function 'clickBPIcon' of SUP_BusinessPolicy_Pane.rb - error: BP icon cannot be found."
    end
  end
  
  #single assign code by type.
  def assigncode(excel_data)
    test_data = excel_data[0].to_s.split(/,/)
    num_row = test_data.size
    result = "ture"
    for i in 0..num_row-1
      before_comment_rows = getcommentrow(test_data[i].to_i)
      #click BP icon.
      clickBPIcon(excel_data[6])
      #select type.
      num_type = clickMarkByType(excel_data[2], excel_data[4])
      if num_type ==1 || excel_data[4].to_i == 1
        #input comment.
        inputComment(excel_data[1])
        clickOK
        #verify the row.
        after_comment_rows = getcommentrow(test_data[i].to_i)
        if after_comment_rows = before_comment_rows+1
          result = "true"
        else
          result = "false"
          puts "error: the row count is incorrect with only one BP."
        end
      else
        for j in 0..num_type-2
          #input comment.
          inputComment(excel_data[1])
          clickOK          
        end
        after_comment_rows = getcommentrow(test_data[i].to_i)
        if after_comment_rows = before_comment_rows+num_type-1
          result = "true"
        else
          result = "false"   
          puts "error: the row count is incorrect when greater 1 BP."       
        end
      end  

      #verify the code name in BP pane.
      codename = getBPCodeName(excel_data[6])
      if codename == excel_data[2].to_s
        result += "true"
      else
        result += "false"
      end

      #verify comment and code.
      mark_result = verifyMark(test_data[i], excel_data[2], excel_data[1], excel_data[5])
      if (mark_result.to_s+result).include?"false"
        return false
      else
        return true
      end
    end    
  end  
  
  
  
  
  
  private
  
  #BP table
  def bp_table
    @browser.div(:id => "BusinessPolicyPane").div(:class =>"gridxMain").div(:class => "gridxBody gridxBodyRowHoverEffect")
  end  
  
  #Applied Code Name.
  def bp_row(irow)
    @browser.div(:id => "BusinessPolicyPane").div(:class =>"gridxMain").div(:class => /gridxRow/, :index => irow).table(:class => "gridxRowTable").td(:class => /gridxCell/, :index => 1).div(:class =>"gridxCellWidget")
  end
  
  #Mark Message icon.
  def bp_mark_message(irow)
    #@browser.div(:id => "BusinessPolicyPane").div(:class =>"gridxMain").div(:class => /gridxRow/, :index => irow).table(:class => "gridxRowTable").td(:class => /gridxCell/, :index => 1).div(:class =>"gridxCellWidget").span(:class =>/dijitDropDownButton/)
    @browser.div(:id => "BusinessPolicyPane").div(:class =>"gridxMain").div(:class => /gridxRow/, :index => irow).table(:class => "gridxRowTable").td(:class => /gridxCell/, :index => 1).div(:class =>"gridxCellWidget").span(:class =>/dijitArrowButtonInner/)
  end
  
  #Check box of BPs.
  def bp_checkbox(irow)
    @browser.div(:id => "BusinessPolicyPane").div(:class =>"gridxMain").div(:class =>"gridxRowHeaderBody").div(:class =>"gridxRowHeaderRow", :index =>irow)
  end
  
  
  
  
end