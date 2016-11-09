require 'test_helper'

class TestCasesControllerTest < ActionController::TestCase
  setup do
    @test_case = test_cases(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:test_cases)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create test_case" do
    assert_difference('TestCase.count') do
      post :create, test_case: { created_by: @test_case.created_by, description: @test_case.description, feature: @test_case.feature, is_automated: @test_case.is_automated, modified_by: @test_case.modified_by, name: @test_case.name, product_id: @test_case.product_id, script_path: @test_case.script_path, source_id: @test_case.source_id, weight: @test_case.weight }
    end

    assert_redirected_to test_case_path(assigns(:test_case))
  end

  test "should show test_case" do
    get :show, id: @test_case
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @test_case
    assert_response :success
  end

  test "should update test_case" do
    patch :update, id: @test_case, test_case: { created_by: @test_case.created_by, description: @test_case.description, feature: @test_case.feature, is_automated: @test_case.is_automated, modified_by: @test_case.modified_by, name: @test_case.name, product_id: @test_case.product_id, script_path: @test_case.script_path, source_id: @test_case.source_id, weight: @test_case.weight }
    assert_redirected_to test_case_path(assigns(:test_case))
  end

  test "should destroy test_case" do
    assert_difference('TestCase.count', -1) do
      delete :destroy, id: @test_case
    end

    assert_redirected_to test_cases_path
  end
end
