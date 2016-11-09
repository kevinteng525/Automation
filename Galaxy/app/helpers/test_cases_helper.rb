module TestCasesHelper
  
  def test_case_source_list
    source_list = TestCase::SOURCE_LIST.collect{|s|[s,TestCase::SOURCE_LIST.index(s)]}
  end
  
  def source_name(test_case)
    TestCase::SOURCE_LIST[test_case.source_id]
  end
  
  def created_by(test_case)
    if test_case.created_by
      User.find(test_case.created_by).name
    else
      ''
    end
  end
  def modified_by(test_case)
    if test_case.modified_by
      User.find(test_case.modified_by).name
    else
      ''
    end
  end
end
